namespace DAL.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ClickCount")]
    public partial class ClickCount
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        public DateTime DateClicked { get; set; }

        public int CategoryId { get; set; }

        public int IsMainCat { get; set; }
    }
}
