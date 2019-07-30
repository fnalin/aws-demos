using System.ComponentModel.DataAnnotations;

namespace fansoftapi.Models.Accounts
{

    public class ChangePasswordModel 
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public string NewPassword { get; set; }
    }
}