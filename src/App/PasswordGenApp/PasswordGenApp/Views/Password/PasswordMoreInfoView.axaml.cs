using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using PasswordGenApp.ViewModels.Password;
using ReactiveUI;

namespace PasswordGenApp.Views.Password;

public partial class PasswordMoreInfoView : ReactiveUserControl<PasswordMoreInfoViewModel>
{
    public PasswordMoreInfoView()
    {
        this.WhenActivated(d => d(ViewModel!.SetTextToClipboardInteraction.RegisterHandler(CopyToClipboardHandler)));
        InitializeComponent();
    }

    private async void CopyToClipboardHandler(IInteractionContext<string, Unit> obj)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null)
            await clipboard.SetTextAsync(obj.Input);
        obj.SetOutput(Unit.Default);
    }
}