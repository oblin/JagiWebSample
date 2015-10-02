using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Crypto
{
    public interface ICryptoProvider
    {
        string Encrypt(string text);
        string Decrypt(string text);
    }
}
