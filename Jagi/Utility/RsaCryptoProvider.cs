using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Jagi.Utility
{
    public static class RsaCryptoProvider
    {
        public static string CreateRSAPublicKey()
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                return rsa.ToXmlString(false);
            }
            catch { return null; }
        }

        public static string CreateRSAPrivateKey()
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                return rsa.ToXmlString(true);
            }
            catch { return null; }
        }

        /// <summary> <BR> 
        /// RSA 加密字串 <BR> 
        /// </summary> <BR> 
        /// <param name="original">原始字串</param> <BR> 
        /// <param name="xmlString">公鑰 xml 字串</param> <BR> 
        /// <returns></returns> <BR> 
        public static string EncryptRSA(string original, string xmlString)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                rsa.FromXmlString(xmlString);
                byte[] s = Encoding.ASCII.GetBytes(original);
                return BitConverter.ToString(rsa.Encrypt(s, false)).Replace("-", string.Empty);
            }
            catch { return original; }
        }
        /// <summary> 
        /// RSA 加密字串 <BR> 
        /// 加密後為 256 Bytes Hex String (128 Byte) <BR> 
        /// </summary> <BR> 
        /// <param name="original">原始字串</param> <BR> 
        /// <param name="parameters">公鑰 RSAParameters 類別</param> <BR> 
        /// <returns></returns> <BR> 
        public static string EncryptRSA(string original, RSAParameters parameters)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(parameters);
                byte[] s = Encoding.ASCII.GetBytes(original);
                return BitConverter.ToString(rsa.Encrypt(s, false)).Replace("-", string.Empty);
            }
            catch { return original; }
        }
        /// <summary> <BR> 
        /// RSA 解密字串 <BR> 
        /// </summary> <BR> /// <param name="hexString">加密後 Hex String</param> <BR> 
        /// <param name="xmlString">私鑰 xml 字串</param> <BR> 
        /// <returns></returns> <BR> 
        public static string DecryptRSA(string hexString, string xmlString)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlString);
                byte[] s = new byte[hexString.Length / 2];
                int j = 0;
                for (int i = 0; i < hexString.Length / 2; i++)
                {
                    s[i] = Byte.Parse(hexString[j].ToString() + hexString[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    j += 2;
                }
                return Encoding.ASCII.GetString(rsa.Decrypt(s, false));
            }
            catch { return hexString; }
        }
        /// <summary> <BR> 
        /// RSA 解密字串 <BR> 
        /// </summary> <BR> 
        /// <param name="hexString">加密後 Hex String</param> <BR> 
        /// <param name="parameters">私鑰 RSAParameters 類別</param> <BR> 
        /// <returns></returns> <BR> 
        public static string DecryptRSA(string hexString, RSAParameters parameters)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(parameters);
                byte[] s = new byte[hexString.Length / 2];
                int j = 0;
                for (int i = 0; i < hexString.Length / 2; i++)
                {
                    s[i] = Byte.Parse(hexString[j].ToString() + hexString[j + 1].ToString(), System.Globalization.NumberStyles.HexNumber);
                    j += 2;
                }
                return Encoding.ASCII.GetString(rsa.Decrypt(s, false));
            }
            catch { return hexString; }
        }


        private static RSACryptoServiceProvider kiditRSAProvider;
        private const string subjectName = "kiditCA";

        public static byte[] GetEncryptString(string crypt)
        {
            if (string.IsNullOrEmpty(crypt))
                return null;

            if (kiditRSAProvider == null)
            {
                X509Certificate2 cert = GetStoreKiditCert(subjectName);
                // kiditRSAProvider = new RSACryptoServiceProvider();  隨機亂數產生，當系統重新啟動後，會再次產生，與需求不符
                kiditRSAProvider = (RSACryptoServiceProvider)cert.PrivateKey;
            }

            byte[] bteCrypt = null;
            byte[] bteResult = null;
            try
            {
                bteCrypt = Encoding.UTF8.GetBytes(crypt);
                bteResult = kiditRSAProvider.Encrypt(bteCrypt, false);
                //return Encoding.UTF8.GetString(bteResult);
                return bteResult;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
        }

        public static string GetDecryptString(byte[] encrypt)
        {
            if (kiditRSAProvider == null)
            {
                X509Certificate2 cert = GetStoreKiditCert(subjectName);
                // kiditRSAProvider = new RSACryptoServiceProvider();  隨機亂數產生，當系統重新啟動後，會再次產生，與需求不符
                kiditRSAProvider = (RSACryptoServiceProvider)cert.PrivateKey;
            }

            // Add for update
            if (encrypt == null)
                return string.Empty;

            //byte[] bteEncrypt = null;
            string strResault = null;
            byte[] bteDecrypt = null;
            try
            {
                //bteEncrypt = Encoding.UTF8.GetBytes(encrypt);
                bteDecrypt = kiditRSAProvider.Decrypt(encrypt, false);
                strResault = Encoding.UTF8.GetString(bteDecrypt);
                return strResault;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
        }

        private static X509Certificate2 GetStoreKiditCert(string certSubjectName)
        {
            X509Certificate2 cert = null;

            X509Store store = new X509Store("MY", StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            //foreach (var item in store.Certificates)
            //{
            //    if (item.SubjectName.Name == certSubjectName)
            //        cert = item;
            //}
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, certSubjectName, false);

            if (certs != null && certs.Count > 0)
            {
                cert = certs[0];
            }

            return cert;
        }
    }
}
