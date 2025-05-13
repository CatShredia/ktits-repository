using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BookApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Book> Books = new ObservableCollection<Book>();
        private Book selectedBook = null;

        public MainWindow()
        {
            InitializeComponent();
            BooksDataGrid.ItemsSource = Books;
            LoadSampleData();
            UpdateStats();
        }

        private void LoadSampleData()
        {
            Books.Add(new Book { Title = "1984", Author = "Джордж Оруэлл", Genre = "Антиутопия", Rating = 4.8, DateAdded = DateTime.Now });
            Books.Add(new Book { Title = "Мастер и Маргарита", Author = "Булгаков", Genre = "Роман", Rating = 4.6, DateAdded = DateTime.Now });
        }

        private void UpdateStats()
        {
            if (Books.Count == 0) return;
            double avgRating = Books.Average(b => b.Rating);
            string topBook = Books.OrderByDescending(b => b.Rating).First().Title;
            StatsTextBlock.Text = $"Рейтинг: {avgRating:F2}\nКниг: {Books.Count}\nПопулярная: {topBook}";
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var book = new Book
            {
                Title = NewBookTitle.Text,
                Author = NewBookAuthor.Text,
                Genre = NewBookGenre.Text,
                Rating = 0,
                DateAdded = DateTime.Now
            };
            Books.Add(book);
            UpdateStats();
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Book book)
            {
                Books.Remove(book);
                UpdateStats();
            }
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            selectedBook = (sender as Button)?.DataContext as Book;
            if (selectedBook != null)
            {
                EditTitleBox.Text = selectedBook.Title;
            }
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook != null)
            {
                selectedBook.Title = EditTitleBox.Text;
                selectedBook.DateEdited = DateTime.Now;
                BooksDataGrid.Items.Refresh();
            }
        }

        private void AddReview_Click(object sender, RoutedEventArgs e)
        {
            string link = ReviewLinkBox.Text;
            string text = ReviewTextBox.Text;
            MessageBox.Show($"Отзыв добавлен к книге: {link}\n{text}");
            ReviewLinkBox.Clear();
            ReviewTextBox.Clear();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            BooksDataGrid.ItemsSource = Books.Where(b => b.Title.ToLower().Contains(query)).ToList();
        }
    }

    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public double Rating { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateEdited { get; set; }
    }
}
