namespace MyPasswordGenLibrary.Interfaces
{
    public interface IPasswordGenerator
    {
        public string GeneratePasswordWithConvertToB64(int size);
        public string GeneratePasswordWithoutConvertToB64(int size);
    }
}