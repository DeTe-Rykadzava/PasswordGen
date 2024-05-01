using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordGenApp.ViewModels.Database;
using PasswordGenApp.ViewModels.Password;

namespace PasswordGenApp.Services.PassowordService;

public interface IPasswordService
{
    public Task<ActionResultViewModel<IEnumerable<PasswordViewModel>>> GetAllPasswordsAsync();
    public Task<ActionResultViewModel<PasswordViewModel>> AddPasswordAsync(string title, string encryptedPassword, string passwordHash, byte[] iv);
    public Task<ActionResultViewModel<bool>> RemovePasswordAsync(int id);
}