using System.ComponentModel.DataAnnotations;

namespace fansoftapi.Models.Accounts
{

    public class ForgotPasswordModel 
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Email { get; set; }
        
    }
}