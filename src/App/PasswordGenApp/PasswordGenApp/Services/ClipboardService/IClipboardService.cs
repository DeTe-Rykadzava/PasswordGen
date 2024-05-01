using System.Threading.Tasks;

namespace PasswordGenApp.Services.ClipboardService;

public interface IClipboardService
{
    public Task<bool> SetTextAsync(string text);
}