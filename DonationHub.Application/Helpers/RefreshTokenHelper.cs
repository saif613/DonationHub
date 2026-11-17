using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Application.Helpers
{
    public static class RefreshTokenHelper
    {
        public static string Generate()
        {
            var random = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(random);
        }

        public static string Hash(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
