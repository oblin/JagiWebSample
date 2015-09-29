using Jagi.Crypto;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;

namespace JagiConsoleSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            使用ServiceLocator讓加解密可以使用預設的密碼進行不需要每次指定密碼();

            Console.ReadLine();
        }

        /// <summary>
        /// 要注意在設定 unity 時候，同時必須要指定 UnityServiceLocator 與使用 nuget commonServiceLocator (參考 Jagi 的 installed nuget)
        /// 否則無法使用 new CryptoProvider() 會報錯誤！
        /// </summary>
        private static void 使用ServiceLocator讓加解密可以使用預設的密碼進行不需要每次指定密碼()
        {
            var container = new UnityContainer();

            var cryptoSetting = new CryptoSetting("12345678", "87654321");
            container.RegisterInstance(typeof(CryptoSetting), cryptoSetting);

            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            var crypto = new CryptoProvider();

            string testText = "It's a test!";
            string encryptText = crypto.Encrypt(testText);

            Console.WriteLine("=== 使用ServiceLocator讓加解密可以使用預設的密碼進行不需要每次指定密碼 ===");
            Console.WriteLine("Encrypt text: {0}", encryptText);
            Console.WriteLine("Original text: {0}", crypto.Decrypt(encryptText));
            Console.WriteLine("=======================================================================");
        }
    }
}