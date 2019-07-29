using System.ComponentModel.DataAnnotations;

namespace fansoftapi.Models.Accounts
{
    public class SignupModel
    {

        [Required(ErrorMessage = "Campo obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress(ErrorMessage = "email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(6, ErrorMessage = "A senha precisa conter ao menos 6 caracteres")]
        public string Password { get; set; }

    }
}