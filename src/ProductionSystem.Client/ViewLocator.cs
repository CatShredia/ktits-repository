using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ProductionSystem.Client.ViewModels;

namespace ProductionSystem.Client;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var fullName = data.GetType().FullName!;
        var viewName = fullName.Replace("ViewModels.", "Views.", StringComparison.Ordinal)
            .Replace("ViewModel", "View", StringComparison.Ordinal);

        var type = typeof(App).Assembly.GetTypes().FirstOrDefault(t => t.FullName == viewName);
        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + viewName };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
