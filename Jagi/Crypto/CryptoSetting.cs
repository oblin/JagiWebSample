
namespace Jagi.Crypto
{
    public class CryptoSetting
    {
        public string Key { get; set; }
        public string Iv { get; set; }

        public CryptoSetting(string key, string iv)
        {
            this.Key = key;
            this.Iv = iv;
        }
    }
}
