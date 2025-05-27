using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFServiceWebRole
{
	public static class EncryptionService
	{
		public static string Encrypt(string data)
		{
            var chars = data.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                chars[i] = Transform(chars[i]);
            return new string(chars);
        }

        private static char Transform(char c)
        {
            if (c >= 'a' && c <= 'z')
                return (char)('a' + (c - 'a' + 13) % 26);
            if (c >= 'A' && c <= 'Z')
                return (char)('A' + (c - 'A' + 13) % 26);
            return c;
        }
    }
}