namespace DAL.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Form")]
    public partial class Form
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Navn { get; set; }

        [StringLength(10)]
        public string SBNummer { get; set; }

        [Required]
        [StringLength(50)]
        public string RegionBygg { get; set; }

        [Required]
        [StringLength(1300)]
        public string Beskrivelse { get; set; }

        [StringLength(10)]
        public string AntallBerort { get; set; }

        public int CategoryId { get; set; }

        public DateTime Date { get; set; }

        public virtual Accounts Accounts { get; set; }

        public virtual Categories Categories { get; set; }
    }
}
