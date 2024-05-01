using System.Reactive;
using System.Threading.Tasks;
using PasswordGenApp.Services.NavigationService;

namespace PasswordGenApp.ViewModels.Core;

public abstract class RoutableViewModelBase : ViewModelBase
{
    public abstract INavigationService RootNavManager { get; protected set; }

    public abstract string ViewModelViewPath { get; }

    public virtual void OnInitialized(INavigationService rootNavManager)
    {
        RootNavManager = rootNavManager;
    }

    public virtual Task OnShowed() { return Task.FromResult(Unit.Default); }
}