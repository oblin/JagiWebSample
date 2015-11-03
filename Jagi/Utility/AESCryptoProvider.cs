using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Jagi.Utility
{
    /// <summary>
    /// 必須要指定 CryptoSetting，或者給予加解密的值，密碼可為任意長度，建議至少長度為16以上的字串
    /// 此算法比 DES 安全，並且速度要快上 10 倍，推薦使用
    /// 另外 key & iv 使用 128 bit or 256 bit 實際上速度差不多
    /// </summary>
    public class AESCryptoProvider : ICryptoProvider
    {
        private const string _key = "jaGi@12312345678";
        private const string _iv = "J@gi490912345678";

        public CryptoSetting crypto { get; set; }

        public AESCryptoProvider()
        {
            //this.Crypto = new CryptoSetting(_key, _iv);
            this.crypto = ServiceLocator.Current.GetInstance<CryptoSetting>();
        }

        public AESCryptoProvider(CryptoSetting setting)
        {
            this.crypto = setting;
        }

        public AESCryptoProvider(string key, string iv)
        {
            this.crypto = new CryptoSetting(key, iv);
        }

        /// <summary>
        /// 主要的加密函數，直接輸入 Encrypt 即可
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            RijndaelManaged AES = new RijndaelManaged();

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] plainTextData = Encoding.Unicode.GetBytes(text);

            byte[] keyData = md5.ComputeHash(Encoding.Unicode.GetBytes(this.crypto.Key));

            byte[] IVData = md5.ComputeHash(Encoding.Unicode.GetBytes(this.crypto.Iv));

            ICryptoTransform transform = AES.CreateEncryptor(keyData, IVData);

            byte[] output = transform.TransformFinalBlock(plainTextData, 0, plainTextData.Length);

            return Convert.ToBase64String(output); 

        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">加密過的文字</param>
        /// <returns>傳回解密文字</returns>
        public string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            byte[] cipherTextData = Convert.FromBase64String(text);

            RijndaelManaged aes = new RijndaelManaged();

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] keyData = md5.ComputeHash(Encoding.Unicode.GetBytes(this.crypto.Key));

            byte[] IVData = md5.ComputeHash(Encoding.Unicode.GetBytes(this.crypto.Iv));

            ICryptoTransform transform = aes.CreateDecryptor(keyData, IVData);

            byte[] output = transform.TransformFinalBlock(cipherTextData, 0, cipherTextData.Length);

            return Encoding.Unicode.GetString(output); 

        }
    }
}
