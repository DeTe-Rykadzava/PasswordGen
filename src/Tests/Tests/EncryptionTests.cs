using System.Security.Cryptography;
using System.Text;
using MyPasswordGenLibrary;
using MyPasswordGenLibrary.Interfaces;
using Xunit.Abstractions;

namespace Tests;

public class EncryptionTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EncryptionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        IPasswordGenerator passwordGenerator = new PasswordGenerator();
        var password = passwordGenerator.GeneratePasswordWithConvertToB64(32);
        _testOutputHelper.WriteLine(password);
        
        var keyString = "Dete1212";
        var hashedKey = BCrypt.Net.BCrypt.HashPassword(keyString);

        var key = Encoding.UTF8.GetBytes(hashedKey);
        
        using var generator = RandomNumberGenerator.Create();
        var iv = new byte[16];
        generator.GetBytes(iv);

        IEncryption encryption = new Encryption();

        var encrypted = encryption.Encrypt(password, key, iv);
        _testOutputHelper.WriteLine(encrypted);
        var decrypted = encryption.Decrypt(encrypted, key, iv);
        Assert.Equal(password, decrypted);
    }
}