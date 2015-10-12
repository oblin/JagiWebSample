
namespace Jagi.Utility
{
    public interface ICryptoProvider
    {
        string Encrypt(string text);
        string Decrypt(string text);
    }
}
