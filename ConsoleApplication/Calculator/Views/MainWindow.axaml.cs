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

        // Handle digits
        if (e.Key >= Key.D0 && e.Key <= Key.D9)
        {
            vm.NumberCommand.Execute(((int)e.Key - (int)Key.D0).ToString());
            e.Handled = true;
        }
        else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        {
            vm.NumberCommand.Execute(((int)e.Key - (int)Key.NumPad0).ToString());
            e.Handled = true;
        }
        // Handle decimal point
        else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
        {
            vm.NumberCommand.Execute(".");
            e.Handled = true;
        }
        // Handle operations
        else if (e.Key == Key.Add)
        {
            vm.OperationCommand.Execute("+");
            e.Handled = true;
        }
        else if (e.Key == Key.Subtract)
        {
            vm.OperationCommand.Execute("-");
            e.Handled = true;
        }
        else if (e.Key == Key.Multiply)
        {
            vm.OperationCommand.Execute("*");
            e.Handled = true;
        }
        else if (e.Key == Key.Divide)
        {
            vm.OperationCommand.Execute("/");
            e.Handled = true;
        }
        // Equals / Enter
        else if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            vm.EqualsCommand.Execute(null);
            e.Handled = true;
        }
        // Clear / Escape
        else if (e.Key == Key.Escape)
        {
            vm.ClearCommand.Execute(null);
            e.Handled = true;
        }
    }
}