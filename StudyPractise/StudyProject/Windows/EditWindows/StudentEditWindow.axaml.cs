using Avalonia.Controls;
using Avalonia.Interactivity;
using StudyProject.Data;
using StudyProject.Windows.ShowTable;
namespace StudyProject.Windows.EditWindows;
public partial class StudentEditWindow : Window
{
    private readonly Student _student;
    private readonly bool _isNew;
    private readonly StudentControl _control;
    public StudentEditWindow(StudentControl control)
    {
        _student = new Student();
        _isNew = true;
        _control = control;
        InitializeComponent();
    }
    public StudentEditWindow(StudentControl control, Student student)
    {
        _student = student ?? throw new System.ArgumentNullException(nameof(student));
        _isNew = false;
        _control = control;
        InitializeComponent();
        RegNumberBox.Text = _student.RegNumber.ToString();
        FullnameBox.Text = _student.Fullname;
        SpecialtyIdBox.Text = _student.IdSpeciality?.ToString();
    }
    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(RegNumberBox.Text))
        {
            ErrorMessage.Text = "Reg Number is required.";
            return;
        }
        if (!int.TryParse(RegNumberBox.Text, out var regNumber))
        {
            ErrorMessage.Text = "Reg Number must be a valid integer.";
            return;
        }
        if (string.IsNullOrWhiteSpace(FullnameBox.Text))
        {
            ErrorMessage.Text = "Fullname is required.";
            return;
        }
        if (!int.TryParse(SpecialtyIdBox.Text, out var specialtyId))
        {
            ErrorMessage.Text = "Specialty ID must be a valid integer.";
            return;
        }
        if (App.DbContext.Specialties.Find(specialtyId) == null)
        {
            ErrorMessage.Text = $"Specialty with ID {specialtyId} does not exist.";
            return;
        }
        _student.RegNumber = regNumber;
        _student.Fullname = FullnameBox.Text.Trim();
        _student.IdSpeciality = specialtyId;
        if (_isNew)
            App.DbContext.Students.Add(_student);
        App.DbContext.SaveChanges();
        _control.RefreshData();
        Close(true);
    }
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}