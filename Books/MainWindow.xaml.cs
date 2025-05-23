using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace BookApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Book> Books = new ObservableCollection<Book>();
        private Book selectedBook = null;
        public Book SelectedBook { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            BooksDataGrid.ItemsSource = Books;
            BooksComboBox.ItemsSource = Books;
            LoadSampleData();
            UpdateStats();
            DataContext = this;
        }

        private void LoadSampleData()
        {
            Books.Add(new Book { Title = "1984", Author = "Джордж Оруэлл", Genre = "Антиутопия", Rating = 4.8, DateAdded = DateTime.Now });
            Books.Add(new Book { Title = "Мастер и Маргарита", Author = "Булгаков", Genre = "Роман", Rating = 4.6, DateAdded = DateTime.Now });
            Books.Add(new Book { Title = "Мастер и Маргарита2", Author = "Булгаков", Genre = "Роман", Rating = 4.2, DateAdded = DateTime.Now });
            Books.Add(new Book { Title = "Мастер и Маргарита3", Author = "Булгаков", Genre = "Роман", Rating = 4.1, DateAdded = DateTime.Now });
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
                EditAuthorBox.Text = selectedBook.Author;
                EditGenreBox.Text = selectedBook.Genre;
                EditRatingSlider.Value = selectedBook.Rating;
            }
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook != null)
            {
                selectedBook.Title = EditTitleBox.Text;
                selectedBook.Author = EditAuthorBox.Text;
                selectedBook.Genre = EditGenreBox.Text;
                selectedBook.Rating = EditRatingSlider.Value;
                selectedBook.DateEdited = DateTime.Now;

                BooksDataGrid.Items.Refresh();
                UpdateStats();

                MessageBox.Show("Изменения сохранены");
            }
            else
            {
                MessageBox.Show("Не выбрана книга для редактирования");
            }
        }

        private void AddReview_Click(object sender, RoutedEventArgs e)
        {
            if (BooksComboBox.SelectedItem is Book selectedBook)
            {
                string reviewText = ReviewTextBox.Text;
                if (!string.IsNullOrWhiteSpace(reviewText))
                {
                    selectedBook.Reviews.Add(reviewText);
                    MessageBox.Show($"Отзыв добавлен к книге: {selectedBook.Title}");
                    ReviewTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Введите текст отзыва");
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу из списка");
            }
        }


        private void ViewReviews_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Book book)
            {
                string reviews = book.Reviews.Count > 0
                    ? string.Join("\n\n", book.Reviews)
                    : "Нет отзывов";
                MessageBox.Show($"Отзывы на книгу \"{book.Title}\":\n\n{reviews}");
            }
        }

        private void ToggleReadStatus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Book book)
            {
                book.IsRead = !book.IsRead;
                BooksDataGrid.Items.Refresh();
            }
        }

        private void SortComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            if (Books == null) return;

            string searchQuery = SearchBox?.Text?.ToLower() ?? "";

            // Фильтрация по поиску (по названию)
            var filtered = Books.Where(b => b.Title.ToLower().Contains(searchQuery));

            // Фильтрация по выбранному полю и значению
            if (FilterFieldComboBox?.SelectedItem is ComboBoxItem filterFieldItem)
            {
                string filterField = filterFieldItem.Tag?.ToString();
                string filterValue = FilterValueTextBox?.Text?.ToLower() ?? "";

                if (!string.IsNullOrWhiteSpace(filterValue))
                {
                    switch (filterField)
                    {
                        case "Title":
                            filtered = filtered.Where(b => b.Title.ToLower().Contains(filterValue));
                            break;
                        case "Author":
                            filtered = filtered.Where(b => b.Author.ToLower().Contains(filterValue));
                            break;
                        case "Genre":
                            filtered = filtered.Where(b => b.Genre.ToLower().Contains(filterValue));
                            break;
                    }
                }
            }

            if (SortComboBox?.SelectedItem is ComboBoxItem sortItem)
            {
                string sortBy = sortItem.Tag?.ToString();

                switch (sortBy)
                {
                    case "Title":
                        filtered = filtered.OrderBy(b => b.Title);
                        break;
                    case "Author":
                        filtered = filtered.OrderBy(b => b.Author);
                        break;
                    case "Rating":
                        filtered = filtered.OrderByDescending(b => b.Rating);
                        break;
                    case "DateAdded":
                        filtered = filtered.OrderByDescending(b => b.DateAdded);
                        break;
                    case "DateEdited":
                        filtered = filtered.OrderByDescending(b => b.DateEdited ?? DateTime.MinValue);
                        break;
                    default:
                        filtered = filtered.OrderBy(b => b.Title);
                        break;
                }
            }
            else
            {
                filtered = filtered.OrderBy(b => b.Title);
            }

            BooksDataGrid.ItemsSource = filtered.ToList();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilterAndSort();
        }


    }

    public class Book : INotifyPropertyChanged
    {
        private string _title;
        private string _author;
        private string _genre;
        private double _rating;
        private bool _isRead;
        public bool IsRead
        {
            get => _isRead;
            set { _isRead = value; OnPropertyChanged(); }
        }


        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Author
        {
            get => _author;
            set { _author = value; OnPropertyChanged(); }
        }

        public string Genre
        {
            get => _genre;
            set { _genre = value; OnPropertyChanged(); }
        }

        public double Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(); }
        }

        public DateTime DateAdded { get; set; }
        public DateTime? DateEdited { get; set; }
        public ObservableCollection<string> Reviews { get; set; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
