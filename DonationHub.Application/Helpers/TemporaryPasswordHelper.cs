using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Application.Helpers
{
    public static class TemporaryPasswordHelper
    {
        public static string Generate(int length = 16)
        {

            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            const string all = upper + lower + digits;

            var password = new List<char>
            {
                 GetRandomChar(upper),
                 GetRandomChar(lower),
                 GetRandomChar(digits)
            };

            while (password.Count < length)
            {
                password.Add(GetRandomChar(all));
            }


            for (int i = password.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);

                (password[i], password[j]) = (password[j], password[i]);
            }

            return new string(password.ToArray());
        }

        private static char GetRandomChar(string characters)
        {
            return characters[RandomNumberGenerator.GetInt32(characters.Length)];
        }
    }
}
