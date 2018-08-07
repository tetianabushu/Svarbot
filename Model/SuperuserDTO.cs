using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
  
    public class SuperuserDTO
    {
        [Required(ErrorMessage = "Oppgi et navn")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Oppgi et passord")]
        public string Password { get; set; }
    }
}
