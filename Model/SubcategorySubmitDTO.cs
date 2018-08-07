using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {
    public class SubcategorySubmitDTO {

        [Required(ErrorMessage = "Oppgi et navn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Oppgi en instruksjon")]
        [StringLength(500, ErrorMessage = "Instruksjon kan være maks 500 tegn")]
        public string Instruction { get; set; }
    }
}
