namespace MyPasswordGenLibrary.Interfaces
{
    public interface IEncryption
    {
        public string Encrypt(string originalText,  byte[] key, byte[] iv);
        public string Decrypt(string encryptedText, byte[] key, byte[] iv);
    }
}