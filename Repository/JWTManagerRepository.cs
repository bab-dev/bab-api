using Contracts;
using Entities;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
	public class JWTManagerRepository : IJWTManagerRepository
	{
		private readonly IConfiguration _configuration;

		public JWTManagerRepository(RepositoryContext repositoryContext, IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateAccessToken(IEnumerable<Claim> claimList, SymmetricSecurityKey key)
		{
			try
			{
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var expireDate = DateTime.Now.AddMinutes(10);
				var tokenOptions = new JwtSecurityToken(
					issuer: _configuration["ValidIssuer"],
					audience: _configuration["ValidAudience"],
					claims: claimList,
					notBefore: DateTime.Now,
					expires: expireDate,
					signingCredentials: creds
				);

				var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
				return accessToken;
			}
			catch (Exception ex)
			{
				throw new Exception($"Something went wrong inside GenerateAccessToken method: {ex.Message}");
			}
		}

		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

		public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken, SymmetricSecurityKey key)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = key,
				ClockSkew = TimeSpan.Zero
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
			JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}
			return principal;
		}

	}
}
