using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JagiWebSample.Areas.Admin.Models
{
    public class CreateUserView
    {
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Password (Again...)")]
        [Required, DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }

        public IDictionary<string, bool> InitialRoles { get; set; }
    }
}