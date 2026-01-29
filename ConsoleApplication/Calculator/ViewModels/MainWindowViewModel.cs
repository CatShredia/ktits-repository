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

        private void OnNumberPressed(string digit)
        {
            // ❌ Mistake 1: Allows multiple decimal points (e.g., "3.14.15")
            if (Display == "0" || Display == "Error")
                Display = digit;
            else
                Display += digit;
        }

        private void OnOperationPressed(string operation)
        {
            // ❌ Mistake 2: No check if display is valid before parsing
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
                    // ❌ Mistake 3: Division by zero not handled
                    result = _lastValue / currentValue;
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
    }

    // Simple relay command
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