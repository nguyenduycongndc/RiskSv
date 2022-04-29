using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Api.Models;
using System.Text.Json;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenTokenController : ControllerBase
    {
        private readonly JwtConfig _jwtConfig;
        private readonly IDatabase _iDb;
        public GenTokenController(IOptionsMonitor<JwtConfig> optionsMonitor, IDatabase redisDb)
        {
            _jwtConfig = optionsMonitor.CurrentValue;
            _iDb = redisDb;
        }
        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("id", user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("roles", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        [HttpPost]
        public IActionResult GenToken(User login)
        {
            //string sGet = _iDb.StringGet($"Auth.UserInfo.admin");

            //var user = JsonSerializer.Deserialize<User>(sGet);
            var user = new User()
            {
                Id = 1,
                UserName = "hoangnv",
                FullName = "Administrator",
                IsActive = true,
                IsDeleted = false,
                RoleId = 1,
                DomainId = 1,
                Email = "hoangnv@admin.com"
            };
            var jwtToken = GenerateJwtToken(user);

            _iDb.StringSet($"Auth.UserInfo.hoangnv", JsonSerializer.Serialize(user));

            return Ok(new AutResult()
            {
                Result = true,
                Token = jwtToken
            });
        }
    }
}
