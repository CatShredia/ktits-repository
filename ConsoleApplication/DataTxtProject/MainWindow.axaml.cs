using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using TestConsoleProject;

namespace DataTxtProject;

public partial class MainWindow : Window
{
    private TextBox _writeFileNameTextBox;
    private TextBox _writeTextTextBox;
    private TextBox _readFileNameTextBox;
    private TextBlock _footerTextBlock;

    public MainWindow()
    {
        InitializeComponent();
        InitializeTabbedInterface();
    }

    private void InitializeTabbedInterface()
    {
        var mainPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        var tabControl = new TabControl
        {
            Margin = new Thickness(10)
        };

        _writeFileNameTextBox = new TextBox
        {
            Watermark = "Название файла",
            Margin = new Thickness(5)
        };

        _writeTextTextBox = new TextBox
        {
            Watermark = "Текст файла",
            Margin = new Thickness(5),
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 150
        };

        var writeButton = new Button
        {
            Content = "OK",
            Margin = new Thickness(5),
            HorizontalAlignment = HorizontalAlignment.Right
        };
        writeButton.Click += WriteButton_Click;

        var writeStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10)
        };
        writeStackPanel.Children.Add(_writeFileNameTextBox);
        writeStackPanel.Children.Add(_writeTextTextBox);
        writeStackPanel.Children.Add(writeButton);

        var writeTabItem = new TabItem
        {
            Header = "Write",
            Content = writeStackPanel
        };

        _readFileNameTextBox = new TextBox
        {
            Watermark = "Название файла",
            Margin = new Thickness(5)
        };

        var readButton = new Button
        {
            Content = "OK",
            Margin = new Thickness(5),
            HorizontalAlignment = HorizontalAlignment.Right
        };
        readButton.Click += ReadButton_Click;

        var readStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10)
        };
        readStackPanel.Children.Add(_readFileNameTextBox);
        readStackPanel.Children.Add(readButton);

        var readTabItem = new TabItem
        {
            Header = "Read",
            Content = readStackPanel
        };

        tabControl.Items.Add(writeTabItem);
        tabControl.Items.Add(readTabItem);

        _footerTextBlock = new TextBlock
        {
            Margin = new Thickness(10),
            TextWrapping = TextWrapping.Wrap,
            Background = Brushes.LightGray,
            Padding = new Thickness(5),
            MinHeight = 40
        };

        mainPanel.Children.Add(tabControl);
        mainPanel.Children.Add(_footerTextBlock);

        Content = mainPanel;
    }

    private void WriteButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string fileName = _writeFileNameTextBox.Text ?? "text.txt";
        if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".txt";
        }
        string data = _writeTextTextBox.Text ?? "";

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fullPath = Path.Combine(desktopPath, fileName);

        var dataExport = new DataExport();
        string resultPath = dataExport.ExportDataToFile(data, fullPath);

        if (resultPath != null)
        {
            _footerTextBlock.Text = $"Файл успешно сохранен: {resultPath}";
        }
        else
        {
            _footerTextBlock.Text = "Ошибка при сохранении файла";
        }
    }

    private void ReadButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string fileName = _readFileNameTextBox.Text;

        if (string.IsNullOrEmpty(fileName))
        {
            _footerTextBlock.Text = "Введите название файла";
            return;
        }

        if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".txt";
        }

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fullPath = Path.Combine(desktopPath, fileName);

        try
        {
            var dataImporter = new DataImporter();
            string content = dataImporter.ImportDateFromTxt(fullPath);
            _footerTextBlock.Text = $"Содержимое файла:\n{content}";
        }
        catch (Exception ex)
        {
            _footerTextBlock.Text = $"Ошибка при чтении файла: {ex.Message}";
        }
    }
}