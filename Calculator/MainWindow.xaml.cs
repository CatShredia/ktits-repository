using System;
using System.Windows;

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

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var value = (string)((System.Windows.Controls.Button)sender).Content;
            if (_operationClicked || InputTextBox.Text == "0")
            {
                InputTextBox.Text = value;
                _operationClicked = false;
            }
            else
            {
                InputTextBox.Text += value;
            }
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            _operation = (string)((System.Windows.Controls.Button)sender).Content;
            if (double.TryParse(InputTextBox.Text, out _firstNumber))
            {
                _operationClicked = true;
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(InputTextBox.Text, out double secondNumber))
            {
                double result = 0;
                try
                {
                    switch (_operation)
                    {
                        case "+":
                            result = _firstNumber + secondNumber;
                            break;
                        case "-":
                            result = _firstNumber - secondNumber;
                            break;
                        case "*":
                            result = _firstNumber * secondNumber;
                            break;
                        case "/":
                            if (secondNumber == 0)
                                throw new DivideByZeroException();
                            result = _firstNumber / secondNumber;
                            break;
                    }

                    ResultTextBlock.Text = $"Result: {result}";
                    InputTextBox.Text = result.ToString();
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
    }
}
