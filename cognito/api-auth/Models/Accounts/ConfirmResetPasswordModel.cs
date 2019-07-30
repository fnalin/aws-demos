using System.ComponentModel.DataAnnotations;

namespace fansoftapi.Models.Accounts
{

    public class ConfirmResetPasswordModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress(ErrorMessage = "email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public string Code { get; set; }


        [Required(ErrorMessage = "Campo obrigatório")]
        public string NewPassword { get; set; }
    }

}