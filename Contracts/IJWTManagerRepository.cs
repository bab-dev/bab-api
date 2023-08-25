using Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface IJWTManagerRepository
	{
		string GenerateAccessToken(IEnumerable<Claim> claimList, SymmetricSecurityKey key);
		string GenerateRefreshToken();
		ClaimsPrincipal GetPrincipalFromExpiredToken(string token, SymmetricSecurityKey key);
	}
}
