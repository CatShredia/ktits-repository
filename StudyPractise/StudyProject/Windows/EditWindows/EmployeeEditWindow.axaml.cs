using Avalonia.Controls;
using Avalonia.Interactivity;
using StudyProject.Data;
using StudyProject.Windows.ShowTable;
namespace StudyProject.Windows.EditWindows;
public partial class EmployeeEditWindow : Window
{
    private readonly Employee _employee;
    private readonly bool _isNew;
    private readonly EmployeeControl _control;
    public EmployeeEditWindow(EmployeeControl control)
    {
        _employee = new Employee();
        _isNew = true;
        _control = control;
        InitializeComponent();
    }
    public EmployeeEditWindow(EmployeeControl control, Employee employee)
    {
        _employee = employee ?? throw new System.ArgumentNullException(nameof(employee));
        _isNew = false;
        _control = control;
        InitializeComponent();
        FullnameBox.Text = _employee.Fullname;
        PositionBox.Text = _employee.PositionEmp;
        SalaryBox.Text = _employee.Salary?.ToString();
        DeptIdBox.Text = _employee.IdDepart?.ToString();
        ChiefBox.Text = _employee.Chief?.ToString();
    }
    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(FullnameBox.Text))
        {
            ErrorMessage.Text = "Fullname is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(PositionBox.Text))
        {
            ErrorMessage.Text = "Position is required.";
            return;
        }
        if (!decimal.TryParse(SalaryBox.Text, out var salary))
        {
            ErrorMessage.Text = "Salary must be a valid decimal.";
            return;
        }
        if (!int.TryParse(DeptIdBox.Text, out var deptId))
        {
            ErrorMessage.Text = "Dept ID must be a valid integer.";
            return;
        }
        if (!int.TryParse(ChiefBox.Text, out var chiefTab) && !string.IsNullOrWhiteSpace(ChiefBox.Text))
        {
            ErrorMessage.Text = "Chief Tab Number must be a valid integer or empty.";
            return;
        }
        if (App.DbContext.Departments.Find(deptId) == null)
        {
            ErrorMessage.Text = $"Department with ID {deptId} does not exist.";
            return;
        }
        if (!string.IsNullOrWhiteSpace(ChiefBox.Text) && App.DbContext.Employees.Find(chiefTab) == null)
        {
            ErrorMessage.Text = $"Chief with Tab Number {chiefTab} does not exist.";
            return;
        }
        _employee.Fullname = FullnameBox.Text.Trim();
        _employee.PositionEmp = PositionBox.Text.Trim();
        _employee.Salary = salary;
        _employee.IdDepart = deptId;
        _employee.Chief = string.IsNullOrWhiteSpace(ChiefBox.Text) ? null : chiefTab;
        if (_isNew)
            App.DbContext.Employees.Add(_employee);
        App.DbContext.SaveChanges();
        _control.RefreshData();
        Close(true);
    }
    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}