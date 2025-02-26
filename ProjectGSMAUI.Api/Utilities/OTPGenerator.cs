using System;
using System.Security.Cryptography;
using System.Text;

namespace ProjectGSMAUI.Api.Utilities
{
    public class OTPGenerator
    {
        public static string GenerateOTP(int length = 6) 
        {
            const string digits = "0123456789";
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] byteOTP = new byte[length];
                rng.GetBytes(byteOTP);
                StringBuilder sb = new StringBuilder(length);
                foreach (byte b in byteOTP)
                {
                    sb.Append(digits[b % digits.Length]);
                }
                return sb.ToString();
            }
        }
    }
}