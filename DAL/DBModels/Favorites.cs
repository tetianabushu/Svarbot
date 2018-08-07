namespace DAL.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Favorites
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Brukernavn { get; set; }

        public int UndercategoryId { get; set; }

        public virtual Accounts Accounts { get; set; }

        public virtual Undercategory Undercategory { get; set; }
    }
}
