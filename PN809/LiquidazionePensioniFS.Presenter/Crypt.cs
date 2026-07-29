using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class Crypt
    {
        public Crypt()
        {
        }

        public static string Decrypt(string src)
        {
            if (src != String.Empty)
            {
                byte[] keyb = { 0, 3, 4, 5, 6, 4, 5, 2, 7, 6, 0, 3, 4, 5, 6, 4, 5, 2, 7, 6, 0, 3, 4, 5, 6, 4, 5, 2, 7, 6, 4, 5 };
                byte[] ivb = { 10, 61, 25, 12, 122, 120, 80, 248, 13, 182, 196, 212, 176, 46, 23, 85 };
                byte[] p = Convert.FromBase64String(src.Replace(' ', '+'));

                byte[] initialText = new Byte[p.Length];

                RijndaelManaged rv = new RijndaelManaged();
                MemoryStream ms = new MemoryStream(p);
                CryptoStream cs = new CryptoStream(ms, rv.CreateDecryptor(keyb, ivb), CryptoStreamMode.Read);

                int totalByteRead = 0;
                try
                {
                    totalByteRead = cs.Read(initialText, 0, initialText.Length);
                }
                finally
                {
                    ms.Close();
                    cs.Close();
                }

                if (totalByteRead > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < initialText.Length; ++i)
                    {
                        sb.Append((char)initialText[i]);
                    }

                    string source = sb.ToString();
                    source = source.Substring(0, source.IndexOf("\0", StringComparison.CurrentCulture));
                    return source;
                }
            }
            return src;
        }

        public static string Encrypt(string src)
        {
            byte[] keyb = { 0, 3, 4, 5, 6, 4, 5, 2, 7, 6, 0, 3, 4, 5, 6, 4, 5, 2, 7, 6, 0, 3, 4, 5, 6, 4, 5, 2, 7, 6, 4, 5 };
            byte[] ivb = { 10, 61, 25, 12, 122, 120, 80, 248, 13, 182, 196, 212, 176, 46, 23, 85 };

            byte[] p = System.Text.Encoding.ASCII.GetBytes(src.ToCharArray());
            byte[] encodedBytes = { };

            MemoryStream ms = new MemoryStream();
            RijndaelManaged rv = new RijndaelManaged();
            CryptoStream cs = new CryptoStream(ms, rv.CreateEncryptor(keyb, ivb), CryptoStreamMode.Write);

            try
            {
                cs.Write(p, 0, p.Length);
                cs.FlushFinalBlock();
                encodedBytes = ms.ToArray();
            }
            finally
            {
                ms.Close();
                cs.Close();
            }

            return Convert.ToBase64String(encodedBytes);
        }
    }
}
