using Database.Models;

namespace PasswordGenApp.ViewModels.Database;

public class PasswordViewModel
{
    private readonly PasswordModel _password;

    public int Id => _password.Id;

    public string Title => _password.Title;

    public string EncryptedPassword => _password.EncryptedPassword;
    
    public string PasswordHash => _password.PasswordHash;

    public byte[] Iv => _password.Iv;
    public PasswordViewModel(PasswordModel password)
    {
        _password = password;
    }
}