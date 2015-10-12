using Jagi.Utility;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestJagi
{
    [TestClass]
    public class CryptoTest
    {
        [TestInitialize]
        public void Setup()
        {
            var container = new UnityContainer();

            var cryptoSetting = new CryptoSetting("12345678123456781234567812345678", "87654321876543218765432187654321");
            container.RegisterInstance(typeof(CryptoSetting), cryptoSetting);

            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestMethod]
        public void Test_BasicDES_Encrypt_With_Decrypt()
        {
            DESCryptoProvider provider = new DESCryptoProvider("12345678", "87654321");
            string text1 = "Test Encrypt string";

            string encrypt1 = provider.Encrypt(text1);
            Assert.IsTrue(encrypt1.Length > text1.Length);

            string text2 = "1234567890123456789012345678901234567890123456789012345678901234567890";
            string encrypt2 = provider.Encrypt(text2);
            Assert.IsTrue(encrypt2.Length > text2.Length);

            string decrypt1 = provider.Decrypt(encrypt1);
            Assert.AreEqual(text1, decrypt1);

            string decrypt2 = provider.Decrypt(encrypt2);
            Assert.AreEqual(text2, decrypt2);
        }

        [TestMethod]
        public void Test_BasicAES_Encrypt_With_Decrypt()
        {
            AESCryptoProvider provider = new AESCryptoProvider("12345678123678", "87654387654321");
            string text1 = "Test Encrypt string";

            string encrypt1 = provider.Encrypt(text1);
            Assert.IsTrue(encrypt1.Length > text1.Length);

            string text2 = "1234567890123456789012345678901234567890123456789012345678901234567890";
            string encrypt2 = provider.Encrypt(text2);
            Assert.IsTrue(encrypt2.Length > text2.Length);

            string decrypt1 = provider.Decrypt(encrypt1);
            Assert.AreEqual(text1, decrypt1);

            string decrypt2 = provider.Decrypt(encrypt2);
            Assert.AreEqual(text2, decrypt2);

            string text3 = "中文字型";
            string encrypt3 = provider.Encrypt(text3);
            string decrypt3 = provider.Decrypt(encrypt3);
            Assert.AreEqual(text3, decrypt3);
        }


        [TestMethod]
        public void Test_ServiceLocator_Inject_Key()
        {
            AESCryptoProvider provider = new AESCryptoProvider();
            string text1 = "Test Encrypt string";

            string encrypt1 = provider.Encrypt(text1);
            Assert.IsTrue(encrypt1.Length > text1.Length);

            string text2 = "1234567890123456789012345678901234567890123456789012345678901234567890";
            string encrypt2 = provider.Encrypt(text2);
            Assert.IsTrue(encrypt2.Length > text2.Length);

            string decrypt1 = provider.Decrypt(encrypt1);
            Assert.AreEqual(text1, decrypt1);

            string decrypt2 = provider.Decrypt(encrypt2);
            Assert.AreEqual(text2, decrypt2);
        }


        /// <summary>
        /// 這裡主要測試 AES key length 128 and 256 哪一個速度比較快；猜測是 128 但事實上不分上下
        /// 因此取消 TestMethod
        /// </summary>
        //[TestMethod]
        public void Test_Performance_AES_KeyLength()
        {
            AESCryptoProvider defaultProvider = new AESCryptoProvider();
            TimeSpan key32 = Utility.Timing(() =>
            {
                EncryptDescryptLoop(defaultProvider);
            });

            TimeSpan key16 = Utility.Timing(() =>
            {
                AESCryptoProvider provider = new AESCryptoProvider("1234567812345678", "8765432187654321");
                EncryptDescryptLoop(provider);
            });

            Assert.IsTrue(key16 < key32);
        }
        
        private void EncryptDescryptLoop(ICryptoProvider provider)
        {
            for (int i = 0; i < 1000; i++)
            {
                string encrypt = provider.Encrypt(i.ToString());
                provider.Decrypt(encrypt);
            }
        }
    }
}
