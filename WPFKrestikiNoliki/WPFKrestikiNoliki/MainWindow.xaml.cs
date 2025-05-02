using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFKrestikiNoliki
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isXTurn = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content != null) return;
            btn.Content = _isXTurn ? "X" : "O";
            _isXTurn = !_isXTurn;
            CheckWinner();
        }
        private void CheckWinner()
        {
            string[,] filed = new string[3, 3];
            int index = 0;
            foreach(Button btn in GameGrid.Children)
            {
                filed[index / 3, index % 3] = btn.Content?.ToString();
                index++;
            }
            for (int i = 0; i < 3; i++)
            {
                if (filed[i, 0] != null && filed[i, 0] == filed[i, 1] && filed[i, 1] == filed[i, 2])
                    ShowWinner(filed[i, 0]);
                if (filed[0, i] != null && filed[0, i] == filed[1, i] && filed[1, i] == filed[2, i])
                        ShowWinner(filed[0, i]);
            }
            if (filed[0, 0] != null && filed[0, 0] == filed[1, 1] && filed[1, 1] == filed[2, 2])
                ShowWinner(filed[0, 0]);
            if (filed[0, 2] != null && filed[0, 2] == filed[1, 1] && filed[1, 1] == filed[2, 0])
                    ShowWinner(filed[0, 2]);
        }
        private void ShowWinner(string winner)
        {
            MessageBox.Show($"{winner} победил!", "Игра окончена");
            foreach(Button btn in GameGrid.Children)
            {
                btn.Content = null;
                _isXTurn = true;
            }
        }
    }
}
