using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Encription.Controllers
{
    [ApiController]
    [Route("")]
    public class BaseController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Encrypt(string plaintext)
        {
            var data = "";

            // Broj i nazivi servisa (nodova) koji ce vrsiti istovremenu enkripciju
            string[] services = new string[]{"serviceone", "servicetwo", "servicethree", "service4", "service5"};
            var numberOfServices = services.Length;
            string[] splitedPlainText = new string[numberOfServices];

            // Podela plain text-a na onoliko delova koliko ima servisa
            var numberOfLettersInPlainText = plaintext.Length;
            if (numberOfLettersInPlainText <= numberOfServices)
            {
                for (int i = 0; i < numberOfLettersInPlainText; i++)
                {
                    splitedPlainText[i] = plaintext[i].ToString();
                }
            }
            else 
            {
                var numberOfLettersInServices = numberOfLettersInPlainText / numberOfServices;
                int length = numberOfLettersInServices;
                for (int i = 0; i < numberOfServices; i++) 
                {
                    if (i == numberOfServices - 1)
                    {
                        length = numberOfLettersInPlainText - i*numberOfLettersInServices;
                    }
                    splitedPlainText[i] = plaintext.Substring(i * numberOfLettersInServices, length);
                }
            }
            
            int iv = 0;
            List<Task<string>> tasks = new List<Task<string>>();

            for (int i=0; i < numberOfServices; i++) 
            {
                if (!string.IsNullOrEmpty(splitedPlainText[i]))
                {
                    tasks.Add(EncryptText(services[i], splitedPlainText[i], iv));
                    iv += splitedPlainText[i].Length;
                }
            }

            //ceka da se svi taskovi zavrse
            await Task.WhenAll(tasks.ToArray());

            //dodaje rezultate taskova u rezultat koji ce na kraju vratiti
            foreach (var t in tasks) 
            {
                data += t.Result;
            }

            return data;
        }

        async Task<string> EncryptText(string serviceUrl, string plaintext, int iv) { 
            string result = "";
            try{
                using var client = new HttpClient  
                {  
                    BaseAddress = new Uri($"http://{serviceUrl}/")  
                };  
                var response = client.GetAsync($"?plaintext={plaintext}&receivedIv={iv}").Result;  

                if (response.IsSuccessStatusCode)  
                {  
                    var jsonTask = response.Content.ReadAsStringAsync();  
                    jsonTask.Wait();  
                    result = jsonTask.Result;
                } 
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
            return result;
        }
    }
}