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
        [HttpGet]
        public string Encrypt(string plaintext)
        {
            Console.WriteLine($"data: {plaintext}");
            string returnResult = "";

            var keyBytes = GetByteArray("2b7e151628aed2a6abf7158809cf4f3c");
            var iv = GetByteArray("f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff");

            using (var am = new Aes128CounterMode(iv))
            {
                try
                {
                    var inputBytes = Encoding.ASCII.GetBytes(plaintext);
                    var result = new byte[inputBytes.Length];
                    using (var ict = am.CreateEncryptor(keyBytes, null))
                    {
                        ict.TransformBlock(inputBytes, 0, inputBytes.Length, result, 0);
                    }
                    returnResult = GetHexString(result);
                    Console.WriteLine("Encrypted:   {0}", returnResult);
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