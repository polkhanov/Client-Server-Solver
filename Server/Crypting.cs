using System;

namespace Cryptography
{
   public class HachCipler
    {
        private static int CHARMOVE = 11;
        public static int CharMove
        {
            get
            {
                return CHARMOVE;
            }
            set
            {
                if (value > 0 || value < 999) CHARMOVE = value;
            }
        }
        public static string EncryptText(String s)
        {
            char[] ch = s.ToCharArray();
            char[] chc = new char[ch.Length + 10];
            Random rnd = new Random();
            for (int i = 0; i < 5; i++)
            {
                chc[i] = (char)rnd.Next(255);
            }
            for (int i = 5, j = 0; i < chc.Length - 5; i++, j++)
            {
                if (i % 2 == 0) chc[i] = (char)(ch[j] + CHARMOVE - j);
                else chc[i] = (char)(ch[j] - CHARMOVE + j);
            }
            for (int i = chc.Length - 5; i < chc.Length; i++)
            {
                chc[i] = (char)rnd.Next(255);
            }
            string encrypt = string.Empty;
            for (int i = 0; i < chc.Length; i++)
            {
                encrypt += chc[i];
            }
            return encrypt;
        }
        public static string DecryptText(String s)
        {
            char[] ch = s.ToCharArray();
            char[] chc = new char[ch.Length - 10];
            for (int i = 5, j = 0; i < ch.Length - 5; i++, j++)
                if (i % 2 == 0) chc[j] = (char)(ch[i] - CHARMOVE + j);
                else chc[j] = (char)(ch[i] + CHARMOVE - j);
            string decrypt = string.Empty;
            for (int i = 0; i < chc.Length; i++)
            {
                decrypt += chc[i];
            }
            return decrypt;
        }
    }
}
