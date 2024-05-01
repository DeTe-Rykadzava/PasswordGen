using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PasswordGenApp.ViewModels.Core;
using Splat;

namespace PasswordGenApp.Services.NavigationService;

public class NavigationService : INavigationService, INotifyPropertyChanged
{
    private readonly IDependencyResolver _locator;
    
    private RoutableViewModelBase? _currentViewModel;
    public RoutableViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set => Set(ref _currentViewModel, value);
    }

    private int _currentIndex = -1;
    
    private readonly List<RoutableViewModelBase> _history = new List<RoutableViewModelBase>();

    public NavigationService(IDependencyResolver locator)
    {
        _locator = locator;
    }
    
    public async Task GoBack()
    {
        if(_currentIndex != 0)
            _history.RemoveAt(_currentIndex);
        _currentIndex -= 1;
        if (_currentIndex < 0)
            _currentIndex = 0;
        CurrentViewModel = _history[_currentIndex];
        if (CurrentViewModel != null)
            await CurrentViewModel.OnShowed();
    }

    public async Task NavigateTo<T>(bool navigateToNew = false)
    {
        var vm = _locator.GetService<T>();
        if(vm == null) return;
        if (vm is not RoutableViewModelBase viewModel) return;
        var historyVm = _history.FirstOrDefault(x => x.ViewModelViewPath == viewModel.ViewModelViewPath);
        _currentIndex += 1;
        if (historyVm == null || navigateToNew)
        {
            if (historyVm != null)
            {
                _history.Remove(historyVm);
                _currentIndex -= 1;
            }
            _history.Insert(_currentIndex, viewModel);
            CurrentViewModel = viewModel;
            CurrentViewModel.OnInitialized(this); 
        }
        else
        {
            _history.Remove(historyVm);
            _currentIndex -= 1;
            _history.Insert(_currentIndex, historyVm);
            CurrentViewModel = historyVm;
        }
        await CurrentViewModel.OnShowed();
    }

    public async Task NavigateTo(RoutableViewModelBase viewModel)
    {
        var historyVm = _history.FirstOrDefault(x => x.ViewModelViewPath == viewModel.ViewModelViewPath);
        _currentIndex += 1;
        if (historyVm != null)
        {
            _history.Remove(historyVm);
            _currentIndex -= 1;
            _history.Insert(_currentIndex, historyVm);
        }
        else
        {
            _history.Insert(_currentIndex, viewModel);
        }
        CurrentViewModel = viewModel;
        CurrentViewModel.OnInitialized(this); 
        await CurrentViewModel.OnShowed();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}