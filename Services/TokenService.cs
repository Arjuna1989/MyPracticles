using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using API.Entity;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
           _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["SecurityKey"]));
            
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim>{
                 new(JwtRegisteredClaimNames.NameId ,user.Id.ToString()),
                  new(JwtRegisteredClaimNames.UniqueName ,user.UserName)

            };

            var creds = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);

            var descriptor = new SecurityTokenDescriptor{
              Subject = new ClaimsIdentity(claims),
              Expires = DateTime.Now.AddDays(1),
              SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}