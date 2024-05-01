using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using PasswordGenApp.ViewModels;
using PasswordGenApp.ViewModels.Core;
using ReactiveUI;
using Splat;

namespace PasswordGenApp;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var dataType = data.GetType();

        var genericType = typeof(IViewFor<>);
        var specificGenericType = genericType.MakeGenericType(dataType);

        var view = Locator.GetLocator().GetService(specificGenericType);
        
        if (view == null)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control?)Activator.CreateInstance(type);
            }
            else
            {
                return new TextBlock() {
                    Text = $"View not found for {dataType}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center};
            }
        }

        // Получаем тип представления
        var viewType = view.GetType();
        
        // Создаем экземпляр представления
        var controlObject = Activator.CreateInstance(viewType);
        
        if (controlObject is Control control)
            return control;
        
        return new TextBlock() {
            Text = "View not Found",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center};
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}