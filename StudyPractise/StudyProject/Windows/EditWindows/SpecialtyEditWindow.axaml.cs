using Avalonia.Controls;
using Avalonia.Interactivity;
using StudyProject.Data;
using StudyProject.Windows.ShowTable;
namespace StudyProject.Windows.EditWindows;
public partial class SpecialtyEditWindow : Window
{
    private readonly Specialty _specialty;
    private readonly bool _isNew;
    private readonly SpecialtyControl _control;
    public SpecialtyEditWindow(SpecialtyControl control)
    {
        _specialty = new Specialty();
        _isNew = true;
        _control = control;
        InitializeComponent();
    }
    public SpecialtyEditWindow(SpecialtyControl control, Specialty specialty)
    {
        _specialty = specialty ?? throw new System.ArgumentNullException(nameof(specialty));
        _isNew = false;
        _control = control;
        InitializeComponent();
        CodeBox.Text = _specialty.Code;
        DirectionBox.Text = _specialty.Direction;
        DeptIdBox.Text = _specialty.IdDepart?.ToString();
    }
    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(CodeBox.Text))
        {
            ErrorMessage.Text = "Code is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(DirectionBox.Text))
        {
            ErrorMessage.Text = "Direction is required.";
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
        _specialty.Code = CodeBox.Text.Trim();
        _specialty.Direction = DirectionBox.Text.Trim();
        _specialty.IdDepart = deptId;
        if (_isNew)
            App.DbContext.Specialties.Add(_specialty);
        App.DbContext.SaveChanges();
        _control.RefreshData();
        Close(true);
    }
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}