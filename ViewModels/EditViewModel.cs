using System.ComponentModel.DataAnnotations;

namespace AspIdentity.Models
{
   public class EditViewModel
   {    
        
        public string? Id { get; set; }
        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

  
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage="Paralolar eşleşmiyor")]
        public string? ConfirmPassword { get; set; } 
   } 
}