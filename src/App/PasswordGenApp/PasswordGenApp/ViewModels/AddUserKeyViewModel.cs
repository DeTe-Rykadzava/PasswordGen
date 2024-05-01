using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Avalonia;
using PasswordGenApp.Services.NavigationService;
using PasswordGenApp.Services.UserKeyStorageService;
using PasswordGenApp.ViewModels.Core;
using PasswordGenApp.ViewModels.Password;
using ReactiveUI;

namespace PasswordGenApp.ViewModels;

public class AddUserKeyViewModel : RoutableViewModelBase
{
    public override string ViewModelViewPath { get; } = "add_key";
    public override INavigationService RootNavManager { get; protected set; } = null!;
    
    // services
    private readonly IUserKeyStorageService _userKeyStorageServiceService;
    
    // fields
    private string _key = string.Empty;

    [Required]
    public string Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    // commands
    public ICommand SaveCommand { get; }
    public ICommand CanselCommand { get; }
    
    public AddUserKeyViewModel(IUserKeyStorageService userKeyStorageServiceService)
    {
        _userKeyStorageServiceService = userKeyStorageServiceService;

        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await _userKeyStorageServiceService.SetKeyAsync(Key);
            await RootNavManager.NavigateTo<PasswordsViewModel>();
        });
        CanselCommand = ReactiveCommand.Create(() =>
        {
            Environment.Exit(0);
        });
    }
}