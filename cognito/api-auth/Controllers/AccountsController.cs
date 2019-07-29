using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using fansoftapi.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace fansoftapi.Controllers
{


    [Route("api/v1/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        public UserManager<CognitoUser> _userManager { get; }
        public CognitoUserPool _pool { get; }

        public AccountController(
                SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
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
    }


}