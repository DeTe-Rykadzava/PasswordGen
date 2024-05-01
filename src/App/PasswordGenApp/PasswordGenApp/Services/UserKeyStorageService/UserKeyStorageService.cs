using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PasswordGenApp.Services.CoreService;

namespace PasswordGenApp.Services.UserKeyStorageService;

public class UserKeyStorageService : IUserKeyStorageService
{
    private static string FileDirectory = "AppData";
    private static string KeyFilename = "data.db";
    
    public async Task<string?> GetKeyAsync()
    {
        try
        {
            var pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileDirectory, KeyFilename);
                
            if (!File.Exists(pathToFile))
                return null;

            var fileText = await File.ReadAllTextAsync(pathToFile);
            return fileText;
        }
        catch (Exception e)
        {
            AppLogger.LogError("Cannot get user key. Message: {Message}. InnerException: {InnerException}", e.Message,
                e.InnerException);
            return null;
        }
    }

    public async Task<string?> SetKeyAsync(string key)
    {
        try
        {
            var pathToFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileDirectory, KeyFilename);
            
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileDirectory)))
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileDirectory));
            
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(key);
            await using var sw = new StreamWriter(pathToFile, false, Encoding.UTF8);
            await sw.WriteAsync(passwordHash);
            return passwordHash;
        }
        catch (Exception e)
        {
            AppLogger.LogError("Cannot set user key. Message: {Message}. InnerException: {InnerException}", e.Message,
                e.InnerException);
        }
        return null;
    }
}