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
        private bool isPaused = false;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled || isPaused)
                return;

            if (!TryGetTime(isWorkTime, out TimeSpan duration))
            {
                MessageBox.Show("Введите корректные числа.");
                return;
            }

            timeLeft = duration;
            TimerText.Text = timeLeft.ToString(@"mm\:ss");
            StageText.Text = isWorkTime ? "Работа" : "Отдых";
            timer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                isPaused = true;
                StageText.Text = "Пауза";
            }
            else if (isPaused)
            {
                timer.Start();
                isPaused = false;
                StageText.Text = isWorkTime ? "Работа" : "Отдых";
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            isPaused = false;
            isWorkTime = true;
            ResetTimer();
            StageText.Text = "Ожидание...";
        }

        private void ResetTimer()
        {
            TimerText.Text = "25:00";
            SessionCountText.Text = $"Завершено сессий: {completedSessions}";
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
                StartButton_Click(null, null);
            }
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

        private void LogSession()
        {
            string logDir = AppDomain.CurrentDomain.BaseDirectory;
            string logPath = System.IO.Path.Combine(logDir, "session_log.txt");
            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Завершена рабочая сессия\n";
            File.AppendAllText(logPath, log);
        }
    }
}
