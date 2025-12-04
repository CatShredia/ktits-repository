using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using StudyProject.Data;
using StudyProject.Windows.ShowTable;

namespace StudyProject.Windows.EditWindows;

public partial class DisciplineEditWindow : Window
{
    private readonly Discipline _discipline;
    private readonly bool _isNew;
    private readonly DisciplineControl? _control;

    public DisciplineEditWindow(DisciplineControl control)
    {
        _discipline = new Discipline();
        _isNew = true;
        _control = control;
        InitializeComponent();
    }

    public DisciplineEditWindow(DisciplineControl control, Discipline discipline)
    {
        _discipline = discipline ?? throw new ArgumentNullException(nameof(discipline));
        _isNew = false;
        _control = control;
        InitializeComponent();
        CodeBox.Text = _discipline.Code?.ToString();
        HoursBox.Text = _discipline.Hours?.ToString();
        NameBox.Text = _discipline.NameDisc;
        DeptIdBox.Text = _discipline.IdDepart?.ToString();
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;

        if (!int.TryParse(CodeBox.Text, out var code))
        {
            ErrorMessage.Text = "Code must be a valid integer.";
            return;
        }

        if (!int.TryParse(HoursBox.Text, out var hours))
        {
            ErrorMessage.Text = "Hours must be a valid integer.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            ErrorMessage.Text = "Name is required.";
            return;
        }

        if (!int.TryParse(DeptIdBox.Text, out var deptId))
        {
            ErrorMessage.Text = "Department ID must be a valid integer.";
            return;
        }

        if (App.DbContext.Departments.Find(deptId) == null)
        {
            ErrorMessage.Text = $"Department with ID {deptId} does not exist.";
            return;
        }

        _discipline.Code = code;
        _discipline.Hours = hours;
        _discipline.NameDisc = NameBox.Text.Trim();
        _discipline.IdDepart = deptId;

        if (_isNew)
            App.DbContext.Disciplines.Add(_discipline);

        App.DbContext.SaveChanges();
        _control?.RefreshData();
        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}