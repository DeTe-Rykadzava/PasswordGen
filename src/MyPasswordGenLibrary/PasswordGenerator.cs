using System;
using System.Security.Cryptography;
using System.Text;
using MyPasswordGenLibrary.Interfaces;

namespace MyPasswordGenLibrary
{
    public class PasswordGenerator : IPasswordGenerator
    {
        public string GeneratePasswordWithConvertToB64(int size)
        {
            try
            {
                using var generator = RandomNumberGenerator.Create();
                var salt = new byte[size];
                generator.GetBytes(salt);
                return Convert.ToBase64String(salt);
            }
            catch (Exception e)
            {
                PasswordGenLibraryLogger.LogError("Error into GeneratePasswordWithConvertToB64. Message: {Message}.\nInnerException: {InnerException}", e.Message, e.InnerException);
                return string.Empty;
            }
        }
        
        public string GeneratePasswordWithoutConvertToB64(int size)
        {
            try
            {
                using var generator = RandomNumberGenerator.Create();
                var salt = new byte[size];
                generator.GetBytes(salt);
                return Encoding.UTF8.GetString(salt);
            }
            catch (Exception e)
            {
                PasswordGenLibraryLogger.LogError("Error into GeneratePasswordWithoutConvertToB64. Message: {Message}.\nInnerException: {InnerException}", e.Message, e.InnerException);
                return string.Empty;
            }
            
        }
    }
}