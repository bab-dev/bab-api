using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IConfiguration _configuration;
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private SymmetricSecurityKey _key;
        public TokenController(IConfiguration config, ILoggerManager logger, IRepositoryWrapper repository)
        {
            _configuration = config;
            _logger = logger;
            _repository = repository;
            _key =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        }
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenRequest tokenRequest)
        {
            if (tokenRequest is null)
            {
                _logger.LogInfo("Invalid client token request");
                return BadRequest("Invalid client token request");
            }
                
            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;

            var principal = _repository.JWTManager.GetPrincipalFromExpiredToken(accessToken, _key);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = _repository.User.GetUserByEmail(username);

            if (user is null)
            {
                _logger.LogInfo("User not found in DB");
                return BadRequest("User not found in DB");
            }
            if (user.RefreshToken != refreshToken )
            {
                _logger.LogInfo("Refresh tokens do not match");
                return BadRequest("Refresh token is different");
            }
            if (user.RefreshTokenExpireDate >= DateTime.Now)
            {
                _logger.LogInfo("Token is still valid");
                return BadRequest("Token is still valid");
            }

            var newAccessToken = _repository.JWTManager.GenerateAccessToken(principal.Claims, _key);
            var newRefreshToken = _repository.JWTManager.GenerateRefreshToken();

            var newExpireDate = DateTime.Now.AddMinutes(5);

            _repository.User.UpdateUserRefreshToken(user, newRefreshToken, newExpireDate); //Update RefreshToken in DB
            _repository.Save();

            return Ok(new AuthenticatedResponseDTO()
            {   Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpireDate = newExpireDate,
                IDUser = user.Id,
                UserRole = user.Role,
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var user = _repository.User.GetUserByEmail(username);

            if (user == null) return BadRequest();

            DateTime? nullDateTime = null;
            _repository.User.UpdateUserRefreshToken(user, "", nullDateTime); //Update RefreshToken in DB
            _repository.Save();

            return NoContent();
        }
    }
}
