using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using StudyProject.Data;
using System;
using StudyProject.Windows.ShowTable;

namespace StudyProject.Windows.EditWindows;

public partial class ExamEditWindow : Window
{
    private readonly Exam _exam;
    private readonly bool _isNew;

    private readonly ExamControl _control;

    public ExamEditWindow(ExamControl control)
    {
        _exam = new Exam();
        _isNew = true;
        _control = control;
        InitializeComponent();
    }

    public ExamEditWindow(ExamControl control, Exam exam)
    {
        _exam = exam ?? throw new ArgumentNullException(nameof(exam));
        _isNew = false;
        _control = control;
        InitializeComponent();

        if (_exam.ExamDate.HasValue)
            ExamDatePicker.SelectedDate = _exam.ExamDate.Value.ToDateTime(TimeOnly.MinValue);

        DisciplineCodeBox.Text = _exam.DisciplineCode?.ToString();
        StudentRegBox.Text = _exam.StudentReg?.ToString();
        ExaminerTabBox.Text = _exam.ExaminerTab?.ToString();
        ClassroomBox.Text = _exam.Classroom;
        GradeBox.Text = _exam.Grade?.ToString();
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        ErrorMessage.Text = string.Empty;

        if (ExamDatePicker.SelectedDate == null)
        {
            ErrorMessage.Text = "Exam Date is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(DisciplineCodeBox.Text))
        {
            ErrorMessage.Text = "Discipline Code is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(StudentRegBox.Text))
        {
            ErrorMessage.Text = "Student Reg Number is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(ExaminerTabBox.Text))
        {
            ErrorMessage.Text = "Examiner Tab Number is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(ClassroomBox.Text))
        {
            ErrorMessage.Text = "Classroom is required.";
            return;
        }
        if (string.IsNullOrWhiteSpace(GradeBox.Text))
        {
            ErrorMessage.Text = "Grade is required.";
            return;
        }

        if (!int.TryParse(DisciplineCodeBox.Text, out var disciplineCode))
        {
            ErrorMessage.Text = "Discipline Code must be a valid integer.";
            return;
        }
        if (!int.TryParse(StudentRegBox.Text, out var studentReg))
        {
            ErrorMessage.Text = "Student Reg Number must be a valid integer.";
            return;
        }
        if (!int.TryParse(ExaminerTabBox.Text, out var examinerTab))
        {
            ErrorMessage.Text = "Examiner Tab Number must be a valid integer.";
            return;
        }
        if (!int.TryParse(GradeBox.Text, out var grade) || grade < 1 || grade > 5)
        {
            ErrorMessage.Text = "Grade must be an integer between 1 and 5.";
            return;
        }

        if (App.DbContext.Disciplines.Find(disciplineCode) == null)
        {
            ErrorMessage.Text = $"Discipline with ID {disciplineCode} does not exist.";
            return;
        }
        if (App.DbContext.Students.Find(studentReg) == null)
        {
            ErrorMessage.Text = $"Student with Reg Number {studentReg} does not exist.";
            return;
        }
        if (App.DbContext.Employees.Find(examinerTab) == null)
        {
            ErrorMessage.Text = $"Employee (Examiner) with Tab Number {examinerTab} does not exist.";
            return;
        }

        _exam.ExamDate = DateOnly.FromDateTime(ExamDatePicker.SelectedDate.Value.DateTime);
        _exam.DisciplineCode = disciplineCode;
        _exam.StudentReg = studentReg;
        _exam.ExaminerTab = examinerTab;
        _exam.Classroom = ClassroomBox.Text.Trim();
        _exam.Grade = grade;

        if (_isNew)
            App.DbContext.Exams.Add(_exam);

        App.DbContext.SaveChanges();
        
        _control.RefreshDate();
        Close(true);
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(false);
}