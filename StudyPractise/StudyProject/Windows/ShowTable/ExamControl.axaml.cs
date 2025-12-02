using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StudyProject.Windows.ShowTable;

public partial class ExamControl : UserControl
{
    public ExamControl()
    {
        InitializeComponent();

        RefreshDate();
    }

    public void RefreshDate()
    {
        var examLists = App.DbContext.Exams
            .ToList();

        ExamDataGrid.ItemsSource = examLists;
    }
}