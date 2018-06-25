using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManager.ViewModels
{
    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "You must enter a username")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You must enter a password")]
        public string Password { get; set; }
    }
}
