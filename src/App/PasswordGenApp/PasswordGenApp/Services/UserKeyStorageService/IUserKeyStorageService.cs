using System.Threading.Tasks;

namespace PasswordGenApp.Services.UserKeyStorageService;

public interface IUserKeyStorageService
{
    public Task<string?> GetKeyAsync();
    public Task<string?> SetKeyAsync(string key);
}