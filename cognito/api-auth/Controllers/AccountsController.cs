using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using fansoftapi.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using fansoftapi.Models.Options;
using Microsoft.Extensions.Options;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;

namespace fansoftapi.Controllers
{


    [Route("api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        private readonly SecuritySettings _securitySettings;

        public AccountController(
                SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool,
                IOptions<SecuritySettings> securitySettings
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
            _securitySettings = securitySettings.Value;
        }


        [HttpPost]
        public async Task<IActionResult> Signup([FromBody]SignupModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _pool.GetUser(model.Email);
            if (!string.IsNullOrEmpty(user.Status))
            {
                ModelState.AddModelError("UserExists", "Já existe um usuário com esse email");
                return BadRequest(ModelState);
            }

            user.Attributes.Add("name", model.Name);
            var createdUser = await _userManager.CreateAsync(user, model.Password);

            if (createdUser.Succeeded) return Ok();

            createdUser.Errors.ToList().ForEach(u => ModelState.AddModelError(u.Code, u.Description));

            return BadRequest(ModelState);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody]ConfirmModel model)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                ModelState.AddModelError("NotFound", "Usuário não encontrado com esse email");
                return BadRequest(ModelState);
            }

            var result = await (_userManager as CognitoUserManager<CognitoUser>)
                    //ConfirmEmailAsync(user, model.Code);
                    .ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);

            if (result.Succeeded) return Ok();

            result.Errors.ToList().ForEach(u => ModelState.AddModelError(u.Code, u.Description));

            return BadRequest(ModelState);


        }


        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody]LoginModel model)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded) {
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
                // Internal Error Server
                // Logar
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