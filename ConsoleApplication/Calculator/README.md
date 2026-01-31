# Calculator (Avalonia + ReactiveUI)

Simple desktop calculator. UI is in the Views and logic in the ViewModel.

- UI
  - [ConsoleApplication/Calculator/Views/MainWindow.axaml](ConsoleApplication/Calculator/Views/MainWindow.axaml) — layout and buttons.
  - [`Calculator.Views.MainWindow`](ConsoleApplication/Calculator/Views/MainWindow.axaml.cs) — window code-behind.
    - [`Calculator.Views.MainWindow.MainWindow_KeyDown`](ConsoleApplication/Calculator/Views/MainWindow.axaml.cs) — keyboard handling.
    - [`Calculator.Views.MainWindow.MainWindow_TextInput`](ConsoleApplication/Calculator/Views/MainWindow.axaml.cs) — text input handling.

- ViewModel
  - [ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
  - [`Calculator.ViewModels.MainWindowViewModel`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
    - Property: [`Calculator.ViewModels.MainWindowViewModel.Display`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
    - Commands:
      - [`Calculator.ViewModels.MainWindowViewModel.NumberCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.OperationCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.EqualsCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.ClearCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.ToggleSignCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.ToggleAdvancedCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.ConstantCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.UnaryOperationCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
    - Key methods:
      - [`Calculator.ViewModels.MainWindowViewModel.OnNumberPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.OnOperationPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.OnEqualsPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.OnClearPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.OnToggleSignPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.ToggleAdvancedMode`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.OnConstantPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
      - [`Calculator.ViewModels.MainWindowViewModel.OnUnaryOperationPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
    - Helper:
      - [`Calculator.ViewModels.Factorial`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs) (private static method in the same file)

- Commands implementation
  - [`Calculator.ViewModels.RelayCommand<T>`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
  - [`Calculator.ViewModels.RelayCommand`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)

How it works (brief)
- Number and decimal buttons call `NumberCommand` → [`OnNumberPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs).
- Operation buttons set `_lastValue` and `_currentOperation` via `OperationCommand` → [`OnOperationPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs).
- Equals runs the selected operation in [`OnEqualsPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs).
- Unary operations (log, ln, factorial, reciprocal, abs) handled by [`OnUnaryOperationPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs).
- Constants (π, e) via [`OnConstantPressed`](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs).
- Keyboard input is routed in [`Calculator.Views.MainWindow`](ConsoleApplication/Calculator/Views/MainWindow.axaml.cs) and triggers the same commands.

Build & run
```bash
dotnet build ConsoleApplication/Calculator/Calculator.csproj
dotnet run --project ConsoleApplication/Calculator/Calculator.csproj
```

Files
- [ConsoleApplication/Calculator/Views/MainWindow.axaml](ConsoleApplication/Calculator/Views/MainWindow.axaml)
- [ConsoleApplication/Calculator/Views/MainWindow.axaml.cs](ConsoleApplication/Calculator/Views/MainWindow.axaml.cs)
- [ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs](ConsoleApplication/Calculator/ViewModels/MainWindowViewModel.cs)
- [ConsoleApplication/Calculator/Calculator.csproj](ConsoleApplication/Calculator/Calculator.csproj)