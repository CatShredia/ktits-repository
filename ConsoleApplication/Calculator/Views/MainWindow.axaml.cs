using System;
using Avalonia.Controls;
using Avalonia.Input;
using Calculator.ViewModels;

namespace Calculator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        if (e.Key >= Key.D0 && e.Key <= Key.D9)
        {
            vm.NumberCommand.Execute(((int)e.Key - (int)Key.D0).ToString());
            e.Handled = true;
            return;
        }

        if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        {
            vm.NumberCommand.Execute(((int)e.Key - (int)Key.NumPad0).ToString());
            e.Handled = true;
            return;
        }

        if (e.Key == Key.OemPeriod || e.Key == Key.Decimal || e.Key == Key.OemComma)
        {
            vm.NumberCommand.Execute(".");
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Add || e.Key == Key.Subtract ||
            e.Key == Key.Multiply || e.Key == Key.Divide)
        {
            string op = e.Key switch
            {
                Key.Add => "+",
                Key.Subtract => "-",
                Key.Multiply => "*",
                Key.Divide => "/",
                _ => null
            };
            vm.OperationCommand.Execute(op);
            e.Handled = true;
        }
        else if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            vm.EqualsCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            vm.ClearCommand.Execute(null);
            e.Handled = true;
        }
    }

    private void MainWindow_TextInput(object sender, TextInputEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        string text = e.Text;
        if (string.IsNullOrEmpty(text) || text.Length != 1)
            return;

        char c = text[0];
        switch (c)
        {
            case '0': case '1': case '2': case '3': case '4':
            case '5': case '6': case '7': case '8': case '9':
                vm.NumberCommand.Execute(c.ToString());
                e.Handled = true;
                break;
            case '.':
            case ',':
                vm.NumberCommand.Execute(".");
                e.Handled = true;
                break;
            case '+':
            case '-':
            case '*':
            case '/':
                vm.OperationCommand.Execute(c.ToString());
                e.Handled = true;
                break;
            case '\r':
                vm.EqualsCommand.Execute(null);
                e.Handled = true;
                break;
        }
    }
}