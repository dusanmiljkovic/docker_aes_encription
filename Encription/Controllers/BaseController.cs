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
            return plaintext;
        }
    }
}