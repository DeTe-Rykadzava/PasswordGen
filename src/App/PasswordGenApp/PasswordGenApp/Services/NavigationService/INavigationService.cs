using System.Threading.Tasks;
using PasswordGenApp.ViewModels.Core;

namespace PasswordGenApp.Services.NavigationService;

public interface INavigationService
{
    public RoutableViewModelBase? CurrentViewModel { get; }
    public Task GoBack();
    public Task NavigateTo<T>(bool navigateToNew = false);
    public Task NavigateTo(RoutableViewModelBase viewModel);
}