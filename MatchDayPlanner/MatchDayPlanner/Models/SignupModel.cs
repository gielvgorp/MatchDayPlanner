using System.ComponentModel.DataAnnotations;

namespace MatchDayPlanner.Models
{
    public class SignupModel
    {
        [Required(ErrorMessage = "Volledige naam is verplicht!")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email adres is verplicht!")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht!")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
