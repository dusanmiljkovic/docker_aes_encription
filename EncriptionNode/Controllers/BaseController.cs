using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using EncriptionNode.AES_CTR;

namespace EncriptionNode.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController : ControllerBase
    {
        byte[] keyBytes = GetByteArray("2b7e151628aed2a6abf7158809cf4f3c");
        byte[] iv;

        // Nodovi ne ispisuju nista kad se pozove poziv za enkriptovanje (osim ako dodje do greske) ako zelis nesto da prikazu mozes da 
        // sklonis neki od postojecih komentara ili da dodas sta ti hoces da ti se ispisuje
        [HttpGet]
        public string Encrypt(string plaintext, int receivedIv)
        {
            iv = GetByteArray(receivedIv.ToString("X32"));
            // Console.WriteLine(iv); - ovde mozes videti IV kako izgleda 

            if (String.IsNullOrEmpty(plaintext))
                return "";
            // Console.WriteLine($"data: {plaintext}"); - ispisuje primljenu porukus
            string returnResult = "";

            using (var am = new Aes128CounterMode(iv))
            {
                try
                {
                    for (int i = 0; i < plaintext.Length; i++) 
                    {
                        var inputBytes = Encoding.ASCII.GetBytes(new char[]{ plaintext[i] });
                        var result = new byte[inputBytes.Length];
                        using (var ict = am.CreateEncryptor(keyBytes, null))
                        {
                            ict.TransformBlock(inputBytes, 0, inputBytes.Length, result, 0);
                        }
                        returnResult += GetHexString(result);
                        // Console.WriteLine("Encrypted:   {0}", returnResult); - ispisuje enkriptovanu poruku
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Exception {ex}");
                }
            }
            return returnResult;
        }
        private static byte[] GetByteArray(string test)
        {
            var plainText2 = new byte[test.Length / 2];
            for (var i = 0; i < test.Length; i = i + 2)
            {
                plainText2[i / 2] = (byte)Convert.ToInt32(test[i] + "" + test[i + 1], 16);
            }

            return plainText2;
        }
        private static string GetHexString(byte[] test)
        {
            var hex = new StringBuilder(test.Length * 2);
            foreach (var b in test)
                hex.Append(b.ToString("X2"));
            return hex.ToString();
        }
    }
}