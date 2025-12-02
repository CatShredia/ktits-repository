using Avalonia.Controls;
using Avalonia.Interactivity;
using StudyProject.Data;
using StudyProject.Windows.ShowTable;
namespace StudyProject.Windows.EditWindows;
public partial class DepartmentEditWindow : Window
{
    private readonly Department _department;
    private readonly bool _isNew;
    private readonly DepartmentControl _control;
    public DepartmentEditWindow(DepartmentControl control)
    {
        _department = new Department();
        _isNew = true;
        _control = control;
        InitializeComponent();
    }
    public DepartmentEditWindow(DepartmentControl control, Department department)
    {
        _department = department ?? throw new System.ArgumentNullException(nameof(department));
        _isNew = false;
        _control = control;
        InitializeComponent();
        CipherBox.Text = _department.Cipher;
        NameBox.Text = _department.NameDepart;
        FacultyIdBox.Text = _department.IdFaculty?.ToString();
    }
    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(CipherBox.Text))
        {
            ErrorMessage.Text = "Cipher is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            ErrorMessage.Text = "Name is required.";
            return;
        }
        if (!int.TryParse(FacultyIdBox.Text, out var facultyId))
        {
            ErrorMessage.Text = "Faculty ID must be a valid integer.";
            return;
        }
        if (App.DbContext.Faculties.Find(facultyId) == null)
        {
            ErrorMessage.Text = $"Faculty with ID {facultyId} does not exist.";
            return;
        }
        _department.Cipher = CipherBox.Text.Trim();
        _department.NameDepart = NameBox.Text.Trim();
        _department.IdFaculty = facultyId;
        if (_isNew)
            App.DbContext.Departments.Add(_department);
        App.DbContext.SaveChanges();
        _control.RefreshData();
        Close(true);
    }
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}