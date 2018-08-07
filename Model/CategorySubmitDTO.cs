using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model {
    public class CategorySubmitDTO {
        public int Id { get; set; }
        [Display(Name = "Kategoritype")]
        [Required(ErrorMessage = "Vennligst velg en kategoritype.")]
        public int CategoryType { get; set; }

        [Display(Name = "Kategorinavn")]
        [Required(ErrorMessage = "Oppgi et kategorinavn.")]
        public string CategoryName { get; set; }

        [Display(Name = "Spørsmål")]
        [Required(ErrorMessage = "Oppgi et spørsmål tittel")]
        public string SubcategoryName { get; set; }

        [StringLength(500, ErrorMessage = "Instruksjon kan ikke være lengre enn 500 bokstaver.")]
        [Display(Name = "Instruksjon")]
        [Required(ErrorMessage = "Oppgi en instruksjon.")]
        public string Instruction { get; set; } 
    }
}
