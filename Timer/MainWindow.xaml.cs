using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private TimeSpan timeLeft;
        private bool isWorkTime = true;
        private int completedSessions = 0;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            ResetTimer();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetTime(isWorkTime, out TimeSpan duration))
            {
                MessageBox.Show("Введите корректные числа.");
                return;
            }

            if (!timer.IsEnabled)
            {
                timeLeft = duration;
                TimerText.Text = timeLeft.ToString(@"mm\:ss");
                StageText.Text = isWorkTime ? "Работа" : "Отдых";
                timer.Start();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            isWorkTime = true;
            ResetTimer();
            StageText.Text = "Ожидание...";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
            TimerText.Text = timeLeft.ToString(@"mm\:ss");

            if (timeLeft.TotalSeconds <= 0)
            {
                timer.Stop();

                if (isWorkTime)
                {
                    completedSessions++;
                    SessionCountText.Text = $"Завершено сессий: {completedSessions}";
                    LogSession();
                    MessageBox.Show("Время отдыха!");
                }
                else
                {
                    MessageBox.Show("Время работать!");
                }

                isWorkTime = !isWorkTime;
                StartButton_Click(null, null); // автозапуск следующего этапа
            }
        }


        private void ResetTimer()
        {
            TimerText.Text = "25:00";
            SessionCountText.Text = $"Завершено сессий: {completedSessions}";
        }

        private void LogSession()
        {
            string log = $"Сессия завершена: {DateTime.Now:yyyy-MM-dd HH:mm}\n";
            File.AppendAllText("sessions.txt", log);
        }

        private bool TryGetTime(bool forWork, out TimeSpan result)
        {
            result = TimeSpan.Zero;

            bool minParsed = int.TryParse(forWork ? WorkMinutesBox.Text : BreakMinutesBox.Text, out int minutes);
            bool secParsed = int.TryParse(forWork ? WorkSecondsBox.Text : BreakSecondsBox.Text, out int seconds);

            if (!minParsed || !secParsed || minutes < 0 || seconds < 0 || seconds >= 60)
                return false;

            result = TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
            return true;
        }
    }
}
