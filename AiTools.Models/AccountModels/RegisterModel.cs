using System;
using System.ComponentModel.DataAnnotations;

namespace AiTools.Models.AccountModels
{
    public class RegisterModel
    {
        [Required]
        [RegularExpression("[a-zA-Zа-яА-Я]+")]
        public string Name { get; set; }
        [Required]
        [RegularExpression("[a-zA-Zа-яА-Я]+")]
        public string SirName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
