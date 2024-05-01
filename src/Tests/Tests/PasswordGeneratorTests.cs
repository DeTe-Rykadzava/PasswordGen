using MyPasswordGenLibrary;
using MyPasswordGenLibrary.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Tests;

public class PasswordGeneratorTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PasswordGeneratorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        IPasswordGenerator passwordGenerator = new PasswordGenerator();
        var password = passwordGenerator.GeneratePasswordWithConvertToB64(32);
        _testOutputHelper.WriteLine(password);
        Assert.False(string.IsNullOrWhiteSpace(password));
    }
    
    [Fact]
    public void Test2()
    {
        var size = 32;
        var passwords = new List<string>();
        IPasswordGenerator passwordGenerator = new PasswordGenerator();
        for (int i = 0; i < 20; i++)
        {
            var password = passwordGenerator.GeneratePasswordWithConvertToB64(size);
            if(password == null)
                continue;
            var sizedPassword = password.Substring(0, size);
            _testOutputHelper.WriteLine(sizedPassword);
            passwords.Add(sizedPassword);
        }

        var notUniquePasswords = new List<string>(); 
        
        foreach (var password in passwords)
        {
            var notUniquePass = passwords.Where(x => x == password);
            if (notUniquePass.Count() > 1)
            {
                notUniquePasswords.Add(password);
            }
        }
        
        Assert.False(notUniquePasswords.Any());
    }
}