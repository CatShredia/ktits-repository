using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private double _firstNumber = 0;
        private string _operation = "";
        private bool _operationClicked = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var value = (string)((Button)sender).Content;

            if (value == "." && InputTextBox.Text.Contains("."))
                return;

            if (_operationClicked || InputTextBox.Text == "0")
            {
                InputTextBox.Text = (value == ".") ? "0." : value;
                _operationClicked = false;
            }
            else
            {
                InputTextBox.Text += value;
            }
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            _operation = (string)((Button)sender).Content;
            if (double.TryParse(InputTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _firstNumber))
            {
                _operationClicked = true;
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(InputTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double secondNumber))
            {
                double result = 0;
                try
                {
                    switch (_operation)
                    {
                        case "+": result = _firstNumber + secondNumber; break;
                        case "-": result = _firstNumber - secondNumber; break;
                        case "*": result = _firstNumber * secondNumber; break;
                        case "/":
                            if (secondNumber == 0)
                                throw new DivideByZeroException();
                            result = _firstNumber / secondNumber;
                            break;
                    }

                    ResultTextBlock.Text = $"Result: {result}";
                    InputTextBox.Text = result.ToString(CultureInfo.InvariantCulture);
                    _operationClicked = true;
                }
                catch (DivideByZeroException)
                {
                    ResultTextBlock.Text = "Error: Division by zero!";
                    InputTextBox.Text = "0";
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = "";
            ResultTextBlock.Text = "";
            _firstNumber = 0;
            _operation = "";
            _operationClicked = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.Key.ToString();

            if (key.StartsWith("D") && key.Length == 2 && char.IsDigit(key[1]))
                Number_Click(CreateButton(key[1].ToString()), null);
            else if (key.StartsWith("NumPad"))
                Number_Click(CreateButton(key.Substring(6)), null);
            else if (key == "Decimal" || key == "OemPeriod" || key == "OemComma")
                Number_Click(CreateButton("."), null);
            else if (key == "Add" || (key == "OemPlus" && Keyboard.Modifiers == ModifierKeys.Shift))
                Operation_Click(CreateButton("+"), null);
            else if (key == "Subtract" || key == "OemMinus")
                Operation_Click(CreateButton("-"), null);
            else if (key == "Multiply")
                Operation_Click(CreateButton("*"), null);
            else if (key == "Divide")
                Operation_Click(CreateButton("/"), null);
            else if (key == "Return" || key == "Enter")
                Equals_Click(null, null);
            else if (key == "Back" || key == "Escape")
                Clear_Click(null, null);
        }

        private Button CreateButton(string content)
        {
            return new Button { Content = content };
        }
    }
}
