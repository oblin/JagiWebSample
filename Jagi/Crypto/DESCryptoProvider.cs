using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Jagi.Crypto
{
    /// <summary>
    /// 必須要指定 CryptoSetting，或者給予加解密的值，缺點：不安全，並且 KEY & IV只能八碼　（６４ｂｉｔ）
    /// 且速度比 AES 慢上八倍，不建議使用，改用 AESCryptoProvider
    /// 如果不給予指定，必須要使用 ServiceLocator 提供預設的做法
    /// </summary>
    public class DESCryptoProvider : ICryptoProvider
    {
        private const string _key = "JagiDeft";
        private const string _iv = "J@gi4909";

        [Dependency("default")]
        public CryptoSetting Crypto { get; set; }

        public DESCryptoProvider()
        {
            //this.Crypto = new CryptoSetting(_key, _iv);
            this.Crypto = ServiceLocator.Current.GetInstance<CryptoSetting>();
        }

        public DESCryptoProvider(CryptoSetting setting)
        {
            this.Crypto = setting;
        }

        public DESCryptoProvider(string key, string iv)
        {
            this.Crypto = new CryptoSetting(key, iv);
        }

        #region ========加密========

        /// <summary>
        /// 主要的加密函數，直接輸入 Encrypt 即可
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Encrypt(text, this.Crypto.Key);
        }
        /// <summary> 
        /// 直接使用文字與密碼進行加密
        /// </summary> 
        /// <param name="Text">加密文字</param> 
        /// <param name="sKey">混合加密的 key 值</param> 
        /// <returns>加密後的文字</returns> 
        protected string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(this.Crypto.Iv);
            
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            string encrypt = string.Empty;
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                encrypt = Convert.ToBase64String(ms.ToArray());
            }
            return encrypt;
        }

        #endregion

        #region ========解密========


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">加密過的文字</param>
        /// <returns>傳回解密文字</returns>
        public string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Decrypt(text, this.Crypto.Key);
        }
        /// <summary> 
        /// 解密文字 
        /// </summary> 
        /// <param name="Text">加密過的文字</param> 
        /// <param name="sKey">密碼</param> 
        /// <returns>傳回解密文字</returns> 
        protected string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] dataByteArry = Convert.FromBase64String(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(this.Crypto.Iv);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(dataByteArry, 0, dataByteArry.Length);
                cs.FlushFinalBlock();
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        #endregion
    }
}
