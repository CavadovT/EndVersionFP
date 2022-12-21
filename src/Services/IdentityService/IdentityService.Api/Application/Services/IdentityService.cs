using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public Task<LoginResponseModel> Login(LoginReguestModel requestModel)
        {
            var claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,requestModel.UserName),
                    new Claim(ClaimTypes.Name,"Tural Cavadov"),
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BlaBlacarSecurtyKeySouldBeLongInf"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirydata = DateTime.Now.AddDays(10);

            var token = new JwtSecurityToken(claims: claims, expires: expirydata, signingCredentials: creds, notBefore: DateTime.Now);

            var encodedJwt=new JwtSecurityTokenHandler().WriteToken(token);

            LoginResponseModel response = new()
            {
                UserName = requestModel.UserName,
                UserToken=encodedJwt,
            };

            return Task.FromResult(response);

        }
    }
}
