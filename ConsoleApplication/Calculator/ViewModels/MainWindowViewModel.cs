using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;

namespace Calculator.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private string _display = "0";
    private double _lastValue;
    private string _currentOperation;
    private bool _isAdvanced;

    public string Display
    {
        get => _display;
        set => this.RaiseAndSetIfChanged(ref _display, value);
    }

    public bool IsAdvanced
    {
        get => _isAdvanced;
        set => this.RaiseAndSetIfChanged(ref _isAdvanced, value);
    }

    public ICommand NumberCommand => new RelayCommand<string>(OnNumberPressed);
    public ICommand OperationCommand => new RelayCommand<string>(OnOperationPressed);
    public ICommand EqualsCommand => new RelayCommand(OnEqualsPressed);
    public ICommand ClearCommand => new RelayCommand(OnClearPressed);
    public ICommand ToggleSignCommand => new RelayCommand(OnToggleSignPressed);
    public ICommand ToggleAdvancedCommand => new RelayCommand(ToggleAdvancedMode);
    public ICommand ConstantCommand => new RelayCommand<string>(OnConstantPressed);
    public ICommand UnaryOperationCommand => new RelayCommand<string>(OnUnaryOperationPressed);

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
        double currentValue = double.Parse(Display); 

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
                result = _lastValue / currentValue;
                break;
            case "^":
                result = Math.Pow(_lastValue, currentValue);
                break;
        }

        Display = result.ToString(); 
    }

    private void OnClearPressed()
    {
        Display = "0";
        _lastValue = 0;
        _currentOperation = null;
    }

    private void OnToggleSignPressed()
    {
        if (Display == "0" || Display == "Error" || string.IsNullOrEmpty(Display))
            return;

        Display = Display.StartsWith("-")
            ? Display.Substring(1)
            : "-" + Display;
    }

    private void ToggleAdvancedMode()
    {
        IsAdvanced = !IsAdvanced;
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
                "reciprocal" => 1.0 / x,
                "log10" => Math.Log10(x),
                "ln" => Math.Log(x),
                "factorial" => Factorial((int)x),
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