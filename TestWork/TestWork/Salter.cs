using System.Security.Cryptography;
using System.Text;

namespace TestWork.Salter
{
    public class Salter
    {
        private const string salt = "some_salt";
        public static string GetHashString(string s)
        {
            string salted = s + salt;
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(salted);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
