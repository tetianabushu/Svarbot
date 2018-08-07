namespace DAL.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Superuser")]
    public partial class Superuser
    {
        [Key]
        [StringLength(10)]
        public string Username { get; set; }

        [Required]
        public byte[] Password { get; set; }
    }
}
