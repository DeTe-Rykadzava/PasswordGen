using Database.DatabaseModels;

namespace Database.Models;

public class PasswordModel
{
    private readonly Password _password;
    
    public int Id => _password.Id;
    
    public string Title => _password.Title;
    
    public string EncryptedPassword => _password.EncryptedPassword;
    
    public string PasswordHash => _password.PasswordHash;
    
    public byte[] Iv => _password.Iv;

    public PasswordModel(Password password)
    {
        _password = password;
    }
}