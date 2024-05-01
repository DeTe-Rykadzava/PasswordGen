using System.ComponentModel.DataAnnotations;
using SQLite;

namespace Database.DatabaseModels;

[Table("Passwords")]
public class Password
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string EncryptedPassword { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public byte[] Iv { get; set; } = null!;
}