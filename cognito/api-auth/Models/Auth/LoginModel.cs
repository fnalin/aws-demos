using System.ComponentModel.DataAnnotations;

namespace fansoftapi.Models.Auth
{
    public class LoginModel 
    {
        [Required(ErrorMessage = "campo obrigatório")]
        public string Email { get; set; }

        [Required(ErrorMessage = "campo obrigatório")]
        public string Password { get; set; }
    }
}