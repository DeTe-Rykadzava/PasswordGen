using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using MsBox.Avalonia.Enums;
using MyPasswordGenLibrary;
using PasswordGenApp.Services.DialogService;
using PasswordGenApp.Services.NavigationService;
using PasswordGenApp.Services.PassowordService;
using PasswordGenApp.Services.UserKeyStorageService;
using PasswordGenApp.ViewModels.Core;
using ReactiveUI;

namespace PasswordGenApp.ViewModels.Password;

public class AddPasswordViewModel: RoutableViewModelBase
{
    public override string ViewModelViewPath { get; } = "add_password";
    public override INavigationService RootNavManager { get; protected set; } = null!;
    
    // services
    private readonly IPasswordService _passwordService;
    private readonly IDialogService _dialogService;
    private readonly IUserKeyStorageService _userKeyStorageService;

    // fields
    private string _title = string.Empty;
    
    [Required]
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private string _password = string.Empty;
    [Required]
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    private int? _size;

    public int? Size
    {
        get => _size;
        set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    // commands

    public ICommand GeneratePasswordCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CanselCommand { get; }

    public AddPasswordViewModel(IPasswordService passwordService, IDialogService dialogService, IUserKeyStorageService userKeyStorageService)
    {
        _passwordService = passwordService;
        _dialogService = dialogService;
        _userKeyStorageService = userKeyStorageService;

        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            using var rng = RandomNumberGenerator.Create();
            var iv = new byte[16];
            rng.GetBytes(iv);

            byte[] keyBytes;
           
            var defaultKey = await _userKeyStorageService.GetKeyAsync();
            if (defaultKey == null)
            {
                await _dialogService.ShowWindowDialogAsync("Error", "Couldn't get the key", icon: Icon.Error);
                return;
            }
            keyBytes = Encoding.UTF8.GetBytes(defaultKey);
            
            var encryption = new Encryption();
            var encryptedPassword = encryption.Encrypt(Password, keyBytes, iv);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);
            
            var addResult = await _passwordService.AddPasswordAsync(Title, encryptedPassword, passwordHash, iv);
            if (!addResult.IsSuccess || addResult.Value == null)
            {
                await _dialogService.ShowWindowDialogAsync("Error", "The password could not be saved",
                    icon: Icon.Error);
                return;
            }

            await _dialogService.ShowWindowDialogAsync("Success", "Success", icon: Icon.Success);

            await RootNavManager.GoBack();
        });

        GeneratePasswordCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Size == null || Size == 0)
            {
                await _dialogService.ShowWindowDialogAsync("Stop", "Size of password not set", icon: Icon.Stop);
                return;
            }

            var generator = new PasswordGenerator();
            var generatedPassword = generator.GeneratePasswordWithConvertToB64(Size.Value);
            Password = generatedPassword;
        });
        
        CanselCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await RootNavManager.GoBack();
        });

    }
    
}