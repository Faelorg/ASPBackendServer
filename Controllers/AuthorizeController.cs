using InterfaceServer.Modal;
using InterfaceServer.Repos;
using InterfaceServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterfaceServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly FileTestContext _context;
        private readonly JwtSettings jwtSettings;
        private readonly IRefreshHandler refresh;
        public AuthorizeController(FileTestContext context, IOptions<JwtSettings> options, IRefreshHandler refresh)
        {
            this._context = context;
            this.jwtSettings = options.Value;
            this.refresh = refresh;
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCred userCred)
        {
            var user = await this._context.Users.FirstOrDefaultAsync(item => item.Login == userCred.login && item.Password == userCred.password);

            if (user != null)
            {
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);
                var tokendesc = new SecurityTokenDescriptor
                {
                    Subject=new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.IdUser),
                        new Claim(ClaimTypes.Role, user.RoleIdRole+"")
                    }),
                    Expires=DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                };
                var token = tokenhandler.CreateToken(tokendesc);

                var finaltoken=tokenhandler.WriteToken(token);
                return Ok(new TokenResponse() { Token = finaltoken, RefreshToken = await refresh.GenerateToken(user.IdUser) })
                ;
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateRefreshToken([FromBody] TokenResponse token)
        {
            var _refresh = await this._context.RefreshTokens.FirstOrDefaultAsync(item => item.RefreshToken1 == token.RefreshToken);

            if (_refresh != null)
            {
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);
                SecurityToken securityToken;
                var principal = tokenhandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                }, out securityToken);

                var _token = securityToken as JwtSecurityToken;

                if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string userId = principal.Identity?.Name;
                    var _existdata = await this._context.RefreshTokens.FirstOrDefaultAsync(
                        item => item.UserId == userId && item.RefreshToken1 == token.RefreshToken);
                    if (_existdata != null)
                    {
                        var newtoken = new JwtSecurityToken(claims:principal.Claims.ToArray(), 
                            expires:DateTime.Now.AddSeconds(30),
                            signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.securityKey))
                            , SecurityAlgorithms.HmacSha256));

                        var _finaltoken = tokenhandler.WriteToken(newtoken);

                        return Ok(new TokenResponse()
                        {
                            Token = _finaltoken,
                            RefreshToken = await this.refresh.GenerateToken(userId)
                        });
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
