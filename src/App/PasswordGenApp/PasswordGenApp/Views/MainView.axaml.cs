using System.Reactive;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using PasswordGenApp.ViewModels;
using ReactiveUI;

namespace PasswordGenApp.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        this.WhenActivated(d => d(ViewModel!.RemoveLoadPanel.RegisterHandler(RemoveLoadPanel)));
        InitializeComponent();
    }

    private void RemoveLoadPanel(IInteractionContext<Unit, Unit> obj)
    {
        // var rootPanel = this.GetControl<Grid>("ContentGrid");
        // var panel = this.GetControl<Panel>("LoadPanel");
        // rootPanel.Children.Remove(panel);
        obj.SetOutput(Unit.Default);
    }
}