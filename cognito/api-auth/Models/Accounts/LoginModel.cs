using System.ComponentModel.DataAnnotations;

namespace fansoftapi.Models.Accounts
{
    public class LoginModel 
    {
        [Required(ErrorMessage = "campo obrigatório")]
        public string Email { get; set; }

        [Required(ErrorMessage = "campo obrigatório")]
        public string Password { get; set; }
    }
}