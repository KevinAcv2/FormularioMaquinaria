using System.ComponentModel.DataAnnotations;

namespace FormularioMaquinaria.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Password { get; set; }
    }
}