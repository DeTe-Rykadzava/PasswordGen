using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;
using Database.Models.Core;

namespace Database.Repositories.Interfaces;

public interface IPasswordRepository
{
    public Task<ActionResultModel<IEnumerable<PasswordModel>>> GetAllPasswordsAsync();
    public Task<ActionResultModel<PasswordModel>> AddPasswordAsync(PasswordAddModel model);
    public Task<ActionResultModel<bool>> RemovePasswordAsync(int id);
}