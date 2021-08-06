using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UkrGuru.ApiHole.Helpers
{
    public class AuthService
    {
        private readonly AppSettings _appSettings;

        public AuthService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string Authenticate([FromBody]string apiholekey)
        {
            // authentication fail if not equal settings
            if (_appSettings.ApiHoleKey != apiholekey) return null;

            // authentication successful so generate jwt token
            return generateJwtToken(_appSettings.ApiHoleEmail);
        }

        private string generateJwtToken(string email)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("LoginEmail", email) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}