using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using PasswordGenApp.Services.CoreService;

namespace PasswordGenApp.Services.DialogService;

public class DialogService : IDialogService
{
    private readonly ContentControl _baseDialogControl;
    
    public DialogService(ContentControl baseDialogControl)
    {
        _baseDialogControl = baseDialogControl;
    }


    public async Task<ButtonResult?> ShowPopupDialogAsync(string title, string content, ButtonEnum button = ButtonEnum.Ok, Icon icon = Icon.None)
    {
        try
        {
            var box = MessageBoxManager.GetMessageBoxStandard(title, content, button, icon,
                WindowStartupLocation.CenterOwner);
            return await box.ShowAsPopupAsync(_baseDialogControl);
        }
        catch (Exception e)
        {
            AppLogger.LogError("error into dialog service.\nMessage\t{Message}.\nInnerException\t{InnerException}", e.Message, e.InnerException);
            return null;
        }
    }
    
    private async Task<ButtonResult?> ShowPopupDialogAsync(IMsBox<ButtonResult> msBox)
    {
        try
        {
            return await msBox.ShowAsPopupAsync(_baseDialogControl);
        }
        catch (Exception e)
        {
            AppLogger.LogError("error into dialog service.\nMessage\t{Message}.\nInnerException\t{InnerException}", e.Message, e.InnerException);
            return null;
        }
    }

    public async Task<ButtonResult?> ShowWindowDialogAsync(string title, string content, ButtonEnum button = ButtonEnum.Ok, Icon icon = Icon.None)
    {
        try
        {
            var box = MessageBoxManager.GetMessageBoxStandard(title, content, button, icon,
                WindowStartupLocation.CenterOwner);
            if (_baseDialogControl is Window window)
                return await box.ShowWindowDialogAsync(window);
            else
                return await ShowPopupDialogAsync(box);
        }
        catch (Exception e)
        {
            AppLogger.LogError("error into dialog service.\nMessage\t{Message}.\nInnerException\t{InnerException}", e.Message, e.InnerException);
            return null;
        }
    }
}