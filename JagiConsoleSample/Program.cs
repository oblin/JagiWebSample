using Jagi.Database;
using Jagi.Database.Cache;
using Jagi.Utility;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Threading;

namespace JagiConsoleSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            使用ServiceLocator讓加解密可以使用預設的密碼進行不需要每次指定密碼();
            //測試EMail();

            測試CodeCache();

            DataContext context = new DataContext();
            var cacheInitializer = new InitColumnCache(context);
            cacheInitializer.Execute();
            var cache = new ColumnsCache();
            var column = cache.Get("TableSchema", "Id");
            Console.WriteLine("Column: {0}'s data type is {1}", column.DisplayName, column.DataType);

            column = cache.Get("TableSchema", "TableName");
            Console.WriteLine("Column: {0}'s string length is {1}", column.DisplayName, column.StringMaxLength);

            Console.ReadLine();
        }

        private static void 測試CodeCache()
        {
            DataContext context = new DataContext();
            var cacheInitializer = new InitCodeCache(context);
            cacheInitializer.Execute();

            var cache = new CodeCache();
            var details = cache.GetCodeDetails("IDIOPA");
            foreach (var detail in details)
            {
                Console.WriteLine("ItemType: {0}, ItemCode:{1}", detail.ItemType, detail.ItemCode);
            }
        }

        private static void SetAutoResetEvent(object eventState)
        {
            Console.WriteLine("Set AutoEvent");
            ((AutoResetEvent)eventState).Set();
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

            var crypto = new DESCryptoProvider();

            string testText = "It's a test!";
            string encryptText = crypto.Encrypt(testText);

            Console.WriteLine("=== 使用ServiceLocator讓加解密可以使用預設的密碼進行不需要每次指定密碼 ===");
            Console.WriteLine("Encrypt text: {0}", encryptText);
            Console.WriteLine("Original text: {0}", crypto.Decrypt(encryptText));
            Console.WriteLine("=======================================================================");
        }

        private static void 測試EMail()
        {
            Mailer email = new Mailer(
                new EmailSetting { Email = "redmine.excelsior@gmail.com", Password = "490910490910" });

            AutoResetEvent autoEvent = new AutoResetEvent(false);

            WaitCallback waitCB = new WaitCallback(SetAutoResetEvent);

            email.SendGMail("oblin228@gmail.com", "test", "Test", waitCB, autoEvent);
        }
    }
}