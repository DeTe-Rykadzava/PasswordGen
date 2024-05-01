using PasswordGenApp.ViewModels.Database;

namespace PasswordGenApp.ViewModels.Password.Factory;

public interface IPasswordMoreInfoFactory
{
    public PasswordMoreInfoViewModel GetPasswordMoreInfo(PasswordViewModel password);
}