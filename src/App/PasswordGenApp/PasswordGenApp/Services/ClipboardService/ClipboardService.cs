using System.Threading.Tasks;
using Avalonia.Input.Platform;

namespace PasswordGenApp.Services.ClipboardService;

public class ClipboardService : IClipboardService
{
    private readonly IClipboard? _clipboardProvider;
    
    public ClipboardService(IClipboard? clipboardProvider)
    {
        _clipboardProvider = clipboardProvider;
    }
    
    public async Task<bool> SetTextAsync(string text)
    {
        if (_clipboardProvider == null)
            return false;

        await _clipboardProvider.SetTextAsync(text);
        return true;
    }
}