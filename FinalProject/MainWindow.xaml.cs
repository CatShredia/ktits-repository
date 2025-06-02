// MainWindow.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RecipeApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly RecipeViewModel _viewModel = new RecipeViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel; // Set the DataContext to the ViewModel
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadRecipes(); // Загрузка данных при загрузке окна
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Model
    public class Recipe
    {
        public string Name { get; set; }
        public string Ingredients { get; set; } // Changed to string for simplicity
        public string Description { get; set; }
        public string Improvements { get; set; }

        public string[] IngredientsList
        {
            get
            {
                return Ingredients?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            }
        }
    }

    // ViewModel
    public class RecipeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Recipe> _recipes = new ObservableCollection<Recipe>();
        private Recipe _selectedRecipe;
        private string _searchText;
        private string _recipeName;
        private string _recipeIngredients;
        private string _recipeDescription;
        private string _recipeImprovements;
        private string _selectedSortOption;
        private ObservableCollection<string> _sortOptions;


        public RecipeViewModel()
        {
            // Initialize SortOptions
            SortOptions = new ObservableCollection<string> { "По названию (А-Я)", "По названию (Я-А)" };
            SelectedSortOption = SortOptions.FirstOrDefault(); // Set default sorting

            EditRecipeCommand = new RelayCommand(EditRecipe);
            SaveRecipeCommand = new RelayCommand(SaveRecipe);

            // Load recipes (example, replace with actual data loading)
            //Recipes.Add(new Recipe { Name = "Суп", Ingredients = "Картофель,Морковь,Лук", Description = "Вкусно", Improvements = "Со сметаной" });
            //Recipes.Add(new Recipe { Name = "Пирог", Ingredients = "Мука,Яйца", Description = "Вкусно", Improvements = "С вишней" });
            // LoadRecipes(); // Move data loading to Loaded event in MainWindow.xaml.cs
        }

        public async Task LoadRecipes()
        {
            // Simulate loading from a database or file
            await Task.Delay(500); // Simulate delay

            // Replace with your actual data loading logic (e.g., from a database)
            Recipes = new ObservableCollection<Recipe>
            {
                new Recipe { Name = "Суп", Ingredients = "Картофель, Морковь, Лук", Description = "Вкусно", Improvements = "Со сметаной" },
                new Recipe { Name = "Пирог", Ingredients = "Мука, Яйца", Description = "Вкусно", Improvements = "С вишней" },
                new Recipe { Name = "Борщ", Ingredients = "Свекла, Капуста, Мясо", Description = "Наваристый", Improvements = "Со сметаной и чесноком" }
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
                    ApplySortingAndFiltering(); // Apply sorting and filtering when recipes change
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

        public ObservableCollection<string> SortOptions
        {
            get => _sortOptions;
            set
            {
                if (_sortOptions != value)
                {
                    _sortOptions = value;
                    OnPropertyChanged(nameof(SortOptions));
                }
            }
        }


        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    OnPropertyChanged(nameof(SelectedRecipe));
                    // When a recipe is selected, load its data into the editing fields:
                    if (value != null)
                    {
                        RecipeName = value.Name;
                        RecipeIngredients = value.Ingredients; // Changed from IngredientsList
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
            if (SelectedRecipe != null)
            {
                SelectedRecipe.Name = RecipeName;
                SelectedRecipe.Ingredients = RecipeIngredients;
                SelectedRecipe.Description = RecipeDescription;
                SelectedRecipe.Improvements = RecipeImprovements;
            }
            else
            {
                // If no recipe is selected, create a new one
                Recipes.Add(new Recipe
                {
                    Name = RecipeName,
                    Ingredients = RecipeIngredients,
                    Description = RecipeDescription,
                    Improvements = RecipeImprovements
                });
            }
            ApplySortingAndFiltering();  // Refresh list after save
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

            // Filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(r => r.Name.ToLower().Contains(SearchText.ToLower()));
            }

            // Sort
            switch (SelectedSortOption)
            {
                case "По названию (А-Я)":
                    filtered = filtered.OrderBy(r => r.Name);
                    break;
                case "По названию (Я-А)":
                    filtered = filtered.OrderByDescending(r => r.Name);
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


    // Simple RelayCommand (for demonstration purposes)
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

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