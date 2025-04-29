using System;
using System.Windows;
using System.Windows.Controls;

namespace CalculatorWPF
{
    public partial class MainWindow : Window
    {
        private double _result = 0;
        private string _operation = "";
        private bool _isNewInput = true;  // Флаг для начала ввода нового числа

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonContent = ((Button)sender).Content.ToString();

            if (_isNewInput)
            {
                ResultTextBox.Text = buttonContent;
                _isNewInput = false;
            }
            else
            {
                if (ResultTextBox.Text == "0" && buttonContent != ".")
                {
                    ResultTextBox.Text = buttonContent;
                }
                else
                {
                    ResultTextBox.Text += buttonContent;
                }
            }
        }

        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_operation))
            {
                EqualsButton_Click(sender, e); // Выполняем предыдущую операцию, если есть
            }

            _operation = ((Button)sender).Content.ToString();
            _result = Convert.ToDouble(ResultTextBox.Text);
            _isNewInput = true; // Сбрасываем для ввода следующего числа
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_operation)) return; // Никакой операции не было выбрано

            double secondNumber;
            if (!double.TryParse(ResultTextBox.Text, out secondNumber))
            {
                ResultTextBox.Text = "Ошибка";
                _operation = "";
                _isNewInput = true;
                return; // Прерываем, если ввод некорректен
            }

            try
            {
                switch (_operation)
                {
                    case "+":
                        _result += secondNumber;
                        break;
                    case "-":
                        _result -= secondNumber;
                        break;
                    case "*":
                        _result *= secondNumber;
                        break;
                    case "/":
                        if (secondNumber == 0)
                        {
                            ResultTextBox.Text = "Деление на ноль";
                            _operation = "";
                            _isNewInput = true;
                            return;
                        }
                        _result /= secondNumber;
                        break;
                }
                ResultTextBox.Text = _result.ToString();
            }
            catch (Exception)
            {
                ResultTextBox.Text = "Ошибка";
            }
            finally
            {
                _operation = "";
                _isNewInput = true; // Сбрасываем для ввода нового числа
            }
        }
    }
}