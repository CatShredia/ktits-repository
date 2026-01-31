using System;
using System.Windows.Input;
using ReactiveUI;

namespace Calculator.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private string _display = "0";
    private double _lastValue;
    private string _currentOperation;

    public string Display
    {
        get => _display;
        set => this.RaiseAndSetIfChanged(ref _display, value);
    }

    public ICommand NumberCommand => new RelayCommand<string>(OnNumberPressed);
    public ICommand OperationCommand => new RelayCommand<string>(OnOperationPressed);
    public ICommand EqualsCommand => new RelayCommand(OnEqualsPressed);
    public ICommand ClearCommand => new RelayCommand(OnClearPressed);
    public ICommand ToggleSignCommand => new RelayCommand(OnToggleSignPressed);
    public ICommand ToggleAdvancedCommand => new RelayCommand(ToggleAdvancedMode);
    public ICommand ConstantCommand => new RelayCommand<string>(OnConstantPressed);
    public ICommand UnaryOperationCommand => new RelayCommand<string>(OnUnaryOperationPressed);

    private bool _isAdvanced;

    public bool IsAdvanced
    {
        get => _isAdvanced;
        set => this.RaiseAndSetIfChanged(ref _isAdvanced, value);
    }

    private void ToggleAdvancedMode()
    {
        IsAdvanced = !IsAdvanced;
    }

    private void OnToggleSignPressed()
    {
        if (Display == "0" || Display == "Error" || string.IsNullOrEmpty(Display))
            return;

        if (Display.StartsWith("-"))
        {
            Display = Display.Substring(1);
        }
        else
        {
            Display = "-" + Display;
        }
    }

    private void OnNumberPressed(string digit)
    {
        if (Display == "0" || Display == "Error")
            Display = digit;
        else
            Display += digit;
    }

    private void OnOperationPressed(string operation)
    {
        _lastValue = double.Parse(Display);
        _currentOperation = operation;
        Display = "0";
    }

    private void OnEqualsPressed()
    {
        if (!double.TryParse(Display, out double currentValue))
        {
            Display = "Error";
            return;
        }

        double result = 0;

        switch (_currentOperation)
        {
            case "+":
                result = _lastValue + currentValue;
                break;
            case "-":
                result = _lastValue - currentValue;
                break;
            case "*":
                result = _lastValue * currentValue;
                break;
            case "/":
                if (currentValue == 0)
                {
                    Display = "Error";
                    return;
                }
                result = _lastValue / currentValue;
                break;
            case "^":
                result = Math.Pow(_lastValue, currentValue);
                break;
            default:
                Display = currentValue.ToString("G15");
                return;
        }

        Display = result.ToString("G15");
    }
    
    private void OnClearPressed()
    {
        Display = "0";
        _lastValue = 0;
        _currentOperation = null;
    }

    private void OnConstantPressed(string constant)
    {
        if (Display == "Error") return;

        double value = constant switch
        {
            "pi" => Math.PI,
            "e" => Math.E,
            _ => 0
        };

        Display = value.ToString("G15");
    }

    private void OnUnaryOperationPressed(string op)
    {
        if (Display == "Error") return;

        try
        {
            if (!double.TryParse(Display, out double x))
            {
                Display = "Error";
                return;
            }

            double result = op switch
            {
                "reciprocal" => x == 0 ? throw new DivideByZeroException() : 1.0 / x,
                "log10" => x <= 0 ? throw new ArgumentException() : Math.Log10(x),
                "ln" => x <= 0 ? throw new ArgumentException() : Math.Log(x),
                "factorial" => x < 0 || x != Math.Floor(x) ? throw new ArgumentException() : Factorial((int)x),
                "abs" => Math.Abs(x),
                _ => throw new NotSupportedException()
            };

            Display = result.ToString("G15");
        }
        catch (Exception)
        {
            Display = "Error";
        }
    }

    private static double Factorial(int n)
    {
        if (n < 0) return double.NaN;
        double result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    public RelayCommand(Action<T> execute) => _execute = execute;
    public bool CanExecute(object parameter) => true;
    public void Execute(object parameter) => _execute((T)parameter);
    public event EventHandler CanExecuteChanged;
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    public RelayCommand(Action execute) => _execute = execute;
    public bool CanExecute(object parameter) => true;
    public void Execute(object parameter) => _execute();
    public event EventHandler CanExecuteChanged;
}