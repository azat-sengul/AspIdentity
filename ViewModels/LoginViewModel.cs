using System.ComponentModel.DataAnnotations;

namespace AspIdentity.Models
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; } = true;
    }
}