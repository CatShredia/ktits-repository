using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using wpf_resipe;

namespace RecipeApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public string htmlResponse;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string exampleRecipe = "Шоколадно-банановые овсяные блинчики\r\nЭти блинчики - отличный способ начать день вкусно и полезно! Они сочетают в себе насыщенный вкус шоколада, сладость банана и пользу овсяных хлопьев. Готовятся очень быстро и легко, а результат порадует и взрослых, и детей. Идеальны для завтрака или здорового перекуса.\r\n\r\nКалорийность (примерная, на 1 порцию из 3 блинчиков): ~350 ккал\r\n\r\nНеобходимые продукты:\r\n\r\n1 спелый банан\r\n1 яйцо\r\n1/2 стакана овсяных хлопьев (мелкого помола)\r\n1 столовая ложка какао-порошка\r\n1/2 чайной ложки разрыхлителя\r\nЩепотка соли\r\nРастительное масло (для жарки)\r\nПо желанию: ягоды, мед, орехи для украшения\r\nШаги по приготовлению:\r\n\r\nБанан разомните вилкой в глубокой миске до состояния пюре.\r\nДобавьте яйцо и тщательно перемешайте.\r\nВсыпьте овсяные хлопья, какао-порошок, разрыхлитель и соль. Хорошо перемешайте, чтобы не было комков. Тесто должно получиться достаточно густым.\r\nРазогрейте сковороду на среднем огне, смажьте небольшим количеством растительного масла.\r\nВыкладывайте тесто ложкой на сковороду, формируя небольшие блинчики.\r\nЖарьте блинчики с каждой стороны по 2-3 минуты, до золотистого цвета.\r\nПодавайте блинчики теплыми, украсив ягодами, медом, орехами или другими любимыми добавками. Приятного аппетита!";

            try
            {
                htmlResponse = await Connection.GetResponseFromAI("Напиши любой рецепт, структура ответа: Название, Небольшое описание (1 абзац), Калорийность, Необходимые продукты, Шаги по приготовлению " + exampleRecipe);
                Console.WriteLine("HTML от AI:\n" + htmlResponse);

                testWebBrowser.NavigateToString(htmlResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обращении к AI: " + ex.Message);
            }
        }
        private readonly RecipeViewModel _viewModel = new RecipeViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;

            // Console.SetOut(new ConsoleWriter(consoleOut));
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadRecipes();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Recipe
    {
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public string Description { get; set; }
        public string Improvements { get; set; }

        public string[] IngredientsList
        {
            get
            {
                if (string.IsNullOrEmpty(Ingredients))
                    return Array.Empty<string>();

                return Ingredients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim())
                                .ToArray();
            }
        }

        public int IngredientsCount => IngredientsList.Length;
    }

    public class RecipeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Recipe> _recipes = new ObservableCollection<Recipe>();
        private Recipe _selectedRecipe;
        private string _searchText;
        private string _ingredientFilter;
        private string _recipeName;
        private string _recipeIngredients;
        private string _recipeDescription;
        private string _recipeImprovements;
        private string _selectedSortOption;

        public RecipeViewModel()
        {
            SortOptions = new ObservableCollection<string> 
            { 
                "По названию (А-Я)", 
                "По названию (Я-А)",
                "По количеству ингредиентов (↑)",
                "По количеству ингредиентов (↓)"
            };
            SelectedSortOption = SortOptions.FirstOrDefault();

            EditRecipeCommand = new RelayCommand(EditRecipe);
            SaveRecipeCommand = new RelayCommand(SaveRecipe);
        }

        public async Task LoadRecipes()
        {
            await Task.Delay(500);
            Recipes = new ObservableCollection<Recipe>
            {
                new Recipe { Name = "Суп", Ingredients = "Картофель, Морковь, Лук", Description = "Вкусно", Improvements = "Со сметаной" },
                new Recipe { Name = "Пирог", Ingredients = "Мука, Яйца, Сахар", Description = "Вкусно", Improvements = "С вишней" },
                new Recipe { Name = "Борщ", Ingredients = "Свекла, Капуста, Мясо, Картофель", Description = "Наваристый", Improvements = "Со сметаной и чесноком" },
                new Recipe { Name = "Омлет", Ingredients = "Яйца, Молоко", Description = "Нежный", Improvements = "С сыром и зеленью" }
            };
            ApplySortingAndFiltering();
        }

        public ObservableCollection<Recipe> Recipes
        {
            get => _recipes;
            set
            {
                if (_recipes != value)
                {
                    _recipes = value;
                    OnPropertyChanged(nameof(Recipes));
                    ApplySortingAndFiltering();
                }
            }
        }

        public ObservableCollection<Recipe> FilteredRecipes { get; } = new ObservableCollection<Recipe>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplySortingAndFiltering();
                }
            }
        }

        public string IngredientFilter
        {
            get => _ingredientFilter;
            set
            {
                if (_ingredientFilter != value)
                {
                    _ingredientFilter = value;
                    OnPropertyChanged(nameof(IngredientFilter));
                    ApplySortingAndFiltering();
                }
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    OnPropertyChanged(nameof(SelectedSortOption));
                    ApplySortingAndFiltering();
                }
            }
        }

        public ObservableCollection<string> SortOptions { get; }

        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    OnPropertyChanged(nameof(SelectedRecipe));
                    if (value != null)
                    {
                        RecipeName = value.Name;
                        RecipeIngredients = value.Ingredients;
                        RecipeDescription = value.Description;
                        RecipeImprovements = value.Improvements;
                    }
                    else
                    {
                        ClearEditFields();
                    }
                }
            }
        }

        public string RecipeName
        {
            get => _recipeName;
            set
            {
                if (_recipeName != value)
                {
                    _recipeName = value;
                    OnPropertyChanged(nameof(RecipeName));
                }
            }
        }

        public string RecipeIngredients
        {
            get => _recipeIngredients;
            set
            {
                if (_recipeIngredients != value)
                {
                    _recipeIngredients = value;
                    OnPropertyChanged(nameof(RecipeIngredients));
                }
            }
        }

        public string RecipeDescription
        {
            get => _recipeDescription;
            set
            {
                if (_recipeDescription != value)
                {
                    _recipeDescription = value;
                    OnPropertyChanged(nameof(RecipeDescription));
                }
            }
        }

        public string RecipeImprovements
        {
            get => _recipeImprovements;
            set
            {
                if (_recipeImprovements != value)
                {
                    _recipeImprovements = value;
                    OnPropertyChanged(nameof(RecipeImprovements));
                }
            }
        }

        public ICommand EditRecipeCommand { get; }
        public ICommand SaveRecipeCommand { get; }

        private void EditRecipe()
        {
            if (SelectedRecipe != null)
            {
                RecipeName = SelectedRecipe.Name;
                RecipeIngredients = SelectedRecipe.Ingredients;
                RecipeDescription = SelectedRecipe.Description;
                RecipeImprovements = SelectedRecipe.Improvements;
            }
        }

        private void SaveRecipe()
        {
            if (string.IsNullOrWhiteSpace(RecipeName))
            {
                MessageBox.Show("Название рецепта не может быть пустым!", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedRecipe != null)
            {
                SelectedRecipe.Name = RecipeName;
                SelectedRecipe.Ingredients = RecipeIngredients;
                SelectedRecipe.Description = RecipeDescription;
                SelectedRecipe.Improvements = RecipeImprovements;
            }
            else
            {
                Recipes.Add(new Recipe
                {
                    Name = RecipeName,
                    Ingredients = RecipeIngredients,
                    Description = RecipeDescription,
                    Improvements = RecipeImprovements
                });
            }
            ApplySortingAndFiltering();
            OnPropertyChanged(nameof(FilteredRecipes));
        }

        private void ClearEditFields()
        {
            RecipeName = "";
            RecipeIngredients = "";
            RecipeDescription = "";
            RecipeImprovements = "";
        }

        private void ApplySortingAndFiltering()
        {
            var filtered = Recipes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(r => 
                    r.Name.ToLower().Contains(SearchText.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(IngredientFilter))
            {
                var filterWords = IngredientFilter.ToLower()
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim());
                
                filtered = filtered.Where(r => 
                    filterWords.All(word => 
                        r.IngredientsList.Any(ing => 
                            ing.ToLower().Contains(word))));
            }

            switch (SelectedSortOption)
            {
                case "По названию (А-Я)":
                    filtered = filtered.OrderBy(r => r.Name);
                    break;
                case "По названию (Я-А)":
                    filtered = filtered.OrderByDescending(r => r.Name);
                    break;
                case "По количеству ингредиентов (↑)":
                    filtered = filtered.OrderBy(r => r.IngredientsCount);
                    break;
                case "По количеству ингредиентов (↓)":
                    filtered = filtered.OrderByDescending(r => r.IngredientsCount);
                    break;
            }

            FilteredRecipes.Clear();
            foreach (var recipe in filtered)
            {
                FilteredRecipes.Add(recipe);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}