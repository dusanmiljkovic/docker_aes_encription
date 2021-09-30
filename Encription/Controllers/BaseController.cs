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
        [HttpPost]
        public string Encrypt(string plaintext)
        {
            var data = "";

            string[] services = new string[]{"serviceone", "servicetwo", "servicethree"};
            var numberOfServices = services.Length;
            string[] splitedPlainText = new string[numberOfServices];

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
            

            for (int i=0; i < numberOfServices; i++) 
            {
                if (splitedPlainText[i] != "")
                {
                    try 
                    {
                        using var client = new HttpClient  
                        {  
                            BaseAddress = new Uri($"http://{services[i]}/")  
                        };  
                
                        var response = client.GetAsync($"?plaintext={splitedPlainText[i]}").Result;  
                        if (response.IsSuccessStatusCode)  
                        {  
                            var jsonTask = response.Content.ReadAsStringAsync();  
                            jsonTask.Wait();  
                            data += jsonTask.Result;
                        }  
                    }
                    catch (Exception ex)  
                    {  
                            Console.WriteLine(ex.Message);  
                    }
                }
            }

            return data;
        }
    }
}