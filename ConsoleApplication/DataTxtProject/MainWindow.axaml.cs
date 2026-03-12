using System;
using System.IO;
using Avalonia.Controls;
using TestConsoleProject;

namespace DataTxtProject;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.FindControl<Button>("WriteButton").Click += WriteButton_Click;
        this.FindControl<Button>("ReadButton").Click += ReadButton_Click;
    }

    private void WriteButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var fileNameTextBox = this.FindControl<TextBox>("WriteFileNameTextBox");
        var textTextBox = this.FindControl<TextBox>("WriteTextTextBox");
        var footerTextBlock = this.FindControl<TextBlock>("FooterTextBlock");

        string fileName = fileNameTextBox.Text ?? "text.txt";
        if (!fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".txt";
        }
        string data = textTextBox.Text ?? "";

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string fullPath = Path.Combine(desktopPath, fileName);

        var dataExport = new DataExport();
        string resultPath = dataExport.ExportDataToFile(data, fullPath);

        if (resultPath != null)
        {
            footerTextBlock.Text = $"Файл успешно сохранен: {resultPath}";
        }
        else
        {
            footerTextBlock.Text = "Ошибка при сохранении файла";
        }
    }

    private void ReadButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var fileNameTextBox = this.FindControl<TextBox>("ReadFileNameTextBox");
        var footerTextBlock = this.FindControl<TextBlock>("FooterTextBlock");

        string fileName = fileNameTextBox.Text;

        if (string.IsNullOrEmpty(fileName))
        {
            footerTextBlock.Text = "Введите название файла";
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
            footerTextBlock.Text = $"Содержимое файла:\n{content}";
        }
        catch (Exception ex)
        {
            footerTextBlock.Text = $"Ошибка при чтении файла: {ex.Message}";
        }
    }
}
