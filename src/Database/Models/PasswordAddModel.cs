using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class PasswordAddModel
{
    [Required]
    public string Title { get; set; } = null!;
    
    [Required]
    public string EncryptedPassword { get; set; } = null!;
    
    [Required]
    public string PasswordHash { get; set; } = null!;
    
    [Required]
    public byte[] Iv { get; set; } = null!;
}