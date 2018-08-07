namespace DAL.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Undercategory")]
    public partial class Undercategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Undercategory()
        {
            Favorites = new HashSet<Favorites>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Undercategory_name { get; set; }

        public int Category_Id { get; set; }

        public int Instruks_Veiledning_Id { get; set; }

        public int? Langinstruks_id { get; set; }

        public virtual Categories Categories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favorites> Favorites { get; set; }

        public virtual Instruks_Veiledning Instruks_Veiledning { get; set; }

        public virtual Langinstruks Langinstruks { get; set; }
    }
}
