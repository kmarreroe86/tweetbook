using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Filters;

namespace Tweetbook.Controllers.V1
{
    [ApiKeyAuth]
    public class SecretApiKeyController : ControllerBase
    {

        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("I have no secrets");
        }
    }
}
