using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CurrencyConverter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CurrencyConverterViewModel();
        }
    }

    public class CurrencyConverterViewModel : INotifyPropertyChanged
    {
        // Курсы валют (можно хранить и загружать из файла или базы данных)
        private const decimal USDRate = 90;
        private const decimal EURRate = 97;
        private const decimal CNYRate = 12;

        private decimal _rubles;
        private string _selectedCurrency;
        private string _result;

        public event PropertyChangedEventHandler PropertyChanged;

        public CurrencyConverterViewModel()
        {
            Currencies = new ObservableCollection<string> { "USD", "EUR", "CNY" };
            SelectedCurrency = "USD";
            ConvertCommand = new RelayCommand(Convert);
        }

        public decimal Rubles
        {
            get { return _rubles; }
            set
            {
                _rubles = value;
                OnPropertyChanged(nameof(Rubles));
                Convert(); // Автоматическое обновление (дополнительно)
            }
        }

        public ObservableCollection<string> Currencies { get; set; }

        public string SelectedCurrency
        {
            get { return _selectedCurrency; }
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged(nameof(SelectedCurrency));
                Convert();  // Автоматическое обновление (дополнительно)
            }
        }

        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public ICommand ConvertCommand { get; private set; }

        private void Convert()
        {
            decimal rate = 0;
            switch (SelectedCurrency)
            {
                case "USD":
                    rate = USDRate;
                    break;
                case "EUR":
                    rate = EURRate;
                    break;
                case "CNY":
                    rate = CNYRate;
                    break;
            }

            decimal convertedAmount = Rubles / rate;
            Result = $"{convertedAmount:F2} {SelectedCurrency}";
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private Action _execute;
        private Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}