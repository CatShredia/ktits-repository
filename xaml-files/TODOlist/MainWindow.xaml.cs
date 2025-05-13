using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ToDoList221
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // коллекция, которая автоматически информирует .xaml, о своем изменении
        public ObservableCollection<TodoItem> Tasks { get; set; } = new ObservableCollection<TodoItem>();
        public MainWindow()
        {
            InitializeComponent();
            TaskListLb.ItemsSource = Tasks;
            UpdateCounter();
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskInputTb.Text))
            {
                Tasks.Add(new TodoItem { Title = TaskInputTb.Text, IsDone = false, DueDate = DueDatePicker.SelectedDate});
                TaskInputTb.Text = string.Empty;
                DueDatePicker.SelectedDate = DateTime.Today;
                UpdateCounter();
            }
        }

        private void DeleteTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.DataContext is TodoItem item)
            {
                Tasks.Remove(item);
                UpdateCounter();
            }
        }
        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
           UpdateCounter();
        }
        public void UpdateCounter()
        {
            int counter = 0;
            foreach(var task in Tasks)
            {
                if (!task.IsDone)
                {
                    counter++;
                }
            }
            CounterTextTbl.Text = $"Осталось дел: {counter}";
        }
    }
    public class TodoItem
    {
        public string Title { get; set; }
        public bool IsDone { get; set; }

        public DateTime? DueDate { get; set; }
    }
    public class DoneToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = (bool)value;
            return isDone ? TextDecorations.Strikethrough : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}