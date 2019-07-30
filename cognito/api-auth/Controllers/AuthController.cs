using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using fansoftapi.Models.Auth;
using fansoftapi.Models.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace fansoftapi.Controllers {

    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly SecuritySettings _securitySettings;

        public AuthController(
                SignInManager<CognitoUser> signInManager,
                UserManager<CognitoUser> userManager,
                IOptions<SecuritySettings> securitySettings
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _securitySettings = securitySettings.Value;
        }



        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody]LoginModel model)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    var token = await gerarJwtAsync(model.Email);
                    return Ok(token);
                }
            }
            catch (UserNotConfirmedException)
            {
                return Unauthorized("Usuário ainda não possui acesso pois seu e-mail ainda não foi confirmado");
            }
            catch (System.Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return new ObjectResult(ex.Message);
            }



            ModelState.AddModelError("Email", "Email ou senha inválidos");
            ModelState.AddModelError("Password", "Email ou senha inválidos");
            return BadRequest(ModelState);
        }

        private async Task<string> gerarJwtAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_securitySettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
               {
                     new Claim("id", user.UserID),
                     new Claim("name", user.Attributes["name"]),
                     new Claim("email", email),
                     
                     //new Claim(ClaimTypes.Role, "Admin")
                     // p/ policy:
                     // new Claim("permissions", policy)
                };

            var token = new JwtSecurityToken(
                    issuer: _securitySettings.Emissor,
                    audience: _securitySettings.ValidoEm,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(5), //DateTime.UtcNow.AddHours(_securitySettings.ExpiracaoHoras)
                    notBefore: DateTime.UtcNow,
                    signingCredentials: creds);



            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }

}