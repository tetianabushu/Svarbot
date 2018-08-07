using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class SkjemaDTO

    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vennligst velg en kategori")]
        [Display(Name = "Kategorinavn")]
        public string CategoryName { get; set; }

        [Key]
        [Required(ErrorMessage = "Mangler saksnummer")]
        [Display(Name ="Saksnummer: ")]
        public int Id { get; set; }

        [Required(ErrorMessage ="Vennligst logg inn for å melde inn saker")]
        [Display(Name ="Brukernavn (initialene): ")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vennligst oppgi SBnummer")]
        [Display(Name ="SBNummer: ")]
        [RegularExpression(@"^SB\d{5}$", ErrorMessage ="SBNummer må oppgis som SB etterfulgt av 5 siffer")]
        public string SBnumber { get; set; }
        
        [Required(ErrorMessage ="Vennligst oppgi din lokasjon")]
        [Display(Name = "Hvor jobber du fra (byggning, region): ")]
        public string Location { get; set; }

        [Required(ErrorMessage ="Beskriv hva slags IT-utfordring du opplever.")]
        [Display(Name ="Kort beskrivelse: ")]
        public string Message { get; set; }

        [Required(ErrorMessage ="Vennligst velg antall berørte")]
        [Display(Name = "Antall personer som opplever utfordring: ")]
        public string UserNr { get; set; }

        [Required]
        public System.DateTime DateSubmitted { get; set; }
    }
}
