using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using fansoftapi.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using fansoftapi.Models.Options;
using Microsoft.Extensions.Options;

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
                    .ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);

            if (result.Succeeded) return Ok();

            result.Errors.ToList().ForEach(u => ModelState.AddModelError(u.Code, u.Description));

            return BadRequest(ModelState);


        }

        // https://github.com/aws/aws-aspnet-cognito-identity-provider/blob/master/docs/5-User%20Management%20-%20Change%20and%20reset%20passwords.md
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            var result = await (_userManager as CognitoUserManager<CognitoUser>)
                    .ChangePasswordAsync(user,model.OldPassword,model.NewPassword);

            if (result.Succeeded) return Ok();

            result.Errors.ToList().ForEach(u => ModelState.AddModelError(u.Code, u.Description));

            return BadRequest(ModelState);
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _userManager.FindByEmailAsync(model.Email);

            var result = await (_userManager as CognitoUserManager<CognitoUser>)
                    .ResetPasswordAsync(user);

           if (result.Succeeded) return Ok();

            result.Errors.ToList().ForEach(u => ModelState.AddModelError(u.Code, u.Description));

            return BadRequest(ModelState);
        }


        [HttpPost("confirmresetpassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody]ConfirmResetPasswordModel model)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                ModelState.AddModelError("NotFound", "Usuário não encontrado com esse email");
                return BadRequest(ModelState);
            }

            var result = await (_userManager as CognitoUserManager<CognitoUser>)
                    .ResetPasswordAsync(user, model.Code, model.NewPassword);

            if (result.Succeeded) return Ok();

            result.Errors.ToList().ForEach(u => ModelState.AddModelError(u.Code, u.Description));

            return BadRequest(ModelState);
        }

    }

}
