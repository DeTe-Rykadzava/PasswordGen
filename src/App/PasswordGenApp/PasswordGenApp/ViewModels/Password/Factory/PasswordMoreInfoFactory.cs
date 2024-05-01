using PasswordGenApp.Services.ClipboardService;
using PasswordGenApp.Services.DialogService;
using PasswordGenApp.Services.UserKeyStorageService;
using PasswordGenApp.ViewModels.Database;

namespace PasswordGenApp.ViewModels.Password.Factory;

public class PasswordMoreInfoFactory : IPasswordMoreInfoFactory
{
    private readonly IDialogService _dialogService;
    private readonly IUserKeyStorageService _userKeyStorageService;
    private readonly IClipboardService _clipboardService;
    
    public PasswordMoreInfoFactory(IDialogService dialogService, IUserKeyStorageService userKeyStorageService, IClipboardService clipboardService)
    {
        _dialogService = dialogService;
        _userKeyStorageService = userKeyStorageService;
        _clipboardService = clipboardService;
    }
    public PasswordMoreInfoViewModel GetPasswordMoreInfo(PasswordViewModel password)
    {
        return new PasswordMoreInfoViewModel(_dialogService, _userKeyStorageService, _clipboardService, password);
    }
}