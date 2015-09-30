using System;
using System.Security.Cryptography;
using System.Text;

namespace Jagi.Crypto
{
    /// <summary>
    /// 使用 RNGCryptoServiceProvider 產生由密碼編譯服務供應者 (CSP) 提供的亂數產生器。
    /// </summary>
    public class RandomPassword
    {
        private RNGCryptoServiceProvider rngp = new RNGCryptoServiceProvider();
        private byte[] rb = new byte[4];

        /// <summary>
        /// 產生一個非負數的亂數
        /// </summary>
        private int Next()
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            if (value < 0) value = -value;
            return value;
        }

        /// <summary>
        /// 產生一個非負數且最大值 max 以下的亂數
        /// </summary>
        /// <param name="max">最大值</param>
        private int Next(int max)
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }

        /// <summary>
        /// 產生一個非負數且最小值在 min 以上最大值在 max 以下的亂數
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        private int Next(int min, int max)
        {
            int value = Next(max - min) + min;
            return value;
        }

        public string GetRandomPassword(int length = 12)
        {
            StringBuilder sb = new StringBuilder();
            char[] chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[this.Next(chars.Length - 1)]);
            }
            string Password = sb.ToString();
            return Password;
        }
    }
}