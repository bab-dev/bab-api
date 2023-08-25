using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/authorize")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public ActionResult<string> Authorize()
        {
            var userEmail = User.Identity.Name;
            return string.Format($"Hello {userEmail}");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("admin")]
        public ActionResult<string> AuthorizeAdmin([FromHeader] string accessToken)
        {
            var value = new AuthenticationHeaderValue("Bearer", accessToken);
            var userEmail = User.Identity.Name;
            return string.Format($"Hello {userEmail}, {value}");
        }

        [Authorize(Roles = "USER")]
        [HttpGet]
        [Route("user")]
        public ActionResult<string> AuthorizeUser([FromHeader] string accessToken)
        {
            var value = new AuthenticationHeaderValue("Bearer", accessToken);
            var userEmail = User.Identity.Name;
            return string.Format($"Hello {userEmail}, {value}");
        }
    }
}
