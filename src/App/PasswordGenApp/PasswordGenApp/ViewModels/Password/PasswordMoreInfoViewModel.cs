using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using MsBox.Avalonia.Enums;
using MyPasswordGenLibrary;
using PasswordGenApp.Services.ClipboardService;
using PasswordGenApp.Services.CoreService;
using PasswordGenApp.Services.DialogService;
using PasswordGenApp.Services.NavigationService;
using PasswordGenApp.Services.UserKeyStorageService;
using PasswordGenApp.ViewModels.Core;
using PasswordGenApp.ViewModels.Database;
using ReactiveUI;
using Splat;

namespace PasswordGenApp.ViewModels.Password;

public class PasswordMoreInfoViewModel : RoutableViewModelBase
{
    public override string ViewModelViewPath { get; } = "password_info";
    public override INavigationService RootNavManager { get; protected set; } = null!;
    
    // services
    private readonly IDialogService _dialogService;
    private readonly IUserKeyStorageService _userKeyStorageService;
    private readonly IClipboardService _clipboardService;
    
    // fields
    public PasswordViewModel Password { get; }

    private string _key = string.Empty;
    
    [Required]
    public string Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    private string _decryptedPassword = string.Empty;

    public string DecryptedPassword
    {
        get => _decryptedPassword;
        private set => this.RaiseAndSetIfChanged(ref _decryptedPassword, value);
    }

    public Interaction<string, Unit> SetTextToClipboardInteraction { get; }

    // commands

    public ICommand DecryptWithDefaultKeyCommand { get; }
    public ICommand CanselCommand { get; }
    public ReactiveCommand<string, Unit> ToClipboardCommand { get; }

    public PasswordMoreInfoViewModel(IDialogService dialogService, IUserKeyStorageService userKeyStorageService, IClipboardService clipboardService, PasswordViewModel password)
    {
        _dialogService = dialogService;
        _userKeyStorageService = userKeyStorageService;
        _clipboardService = clipboardService;
        Password = password;
        SetTextToClipboardInteraction = new Interaction<string, Unit>();
        
        DecryptWithDefaultKeyCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            DecryptedPassword = "";
            
            try
            {
                var encryption = new Encryption();
    
                var defaultKey = await _userKeyStorageService.GetKeyAsync();
                if (defaultKey == null )
                {
                    await _dialogService.ShowWindowDialogAsync("Stop", "The standard key could not be obtained", icon: Icon.Stop);
                    return;
                }
    
                if (string.IsNullOrWhiteSpace(Key))
                {
                    await _dialogService.ShowWindowDialogAsync("Stop", "You have not entered the key", icon: Icon.Stop);
                    return;
                }
            
                if (!BCrypt.Net.BCrypt.Verify(Key, defaultKey))
                {
                    await _dialogService.ShowWindowDialogAsync("Stop", "You entered the wrong key", icon: Icon.Stop);
                    return;
                }

                var keyBytes = Encoding.UTF8.GetBytes(defaultKey);
                var decryptedPassword = encryption.Decrypt(Password.EncryptedPassword, keyBytes, Password.Iv);
                DecryptedPassword = decryptedPassword;
            }
            catch (Exception e)
            {
                await _dialogService.ShowWindowDialogAsync("Error", "Something error with decrypt password", icon: Icon.Error);
                AppLogger.LogError("Error with decrypt password. Message: {Message}. InnerException: {InnerException}",
                    e.Message, e.InnerException);
            }
        });
        
        ToClipboardCommand = ReactiveCommand.CreateFromTask(async (string text) =>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                await _dialogService.ShowWindowDialogAsync("Stop", "There is nothing to copy", icon: Icon.Stop);
                return;
            }

            if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime)
            {
                try
                {
                    await SetTextToClipboardInteraction.Handle(text);
                }
                catch (Exception e)
                {
                    await _dialogService.ShowWindowDialogAsync("Fail", "Data could not be copied to the clipboard", icon: Icon.Info);
                }
            }
            else
            {
                var result = await _clipboardService.SetTextAsync(text);
                if (!result)
                {
                    await _dialogService.ShowWindowDialogAsync("Fail", "Data could not be copied to the clipboard", icon: Icon.Info);
                }
            }
        });
        
        CanselCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await RootNavManager.GoBack();
        });
    }
}