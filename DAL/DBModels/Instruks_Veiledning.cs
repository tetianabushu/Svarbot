namespace DAL.DBModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Instruks_Veiledning
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Instruks_Veiledning()
        {
            Undercategory = new HashSet<Undercategory>();
        }

        public int Id { get; set; }

        [StringLength(500)]
        public string Inskruks_beskrivelse { get; set; }

        [StringLength(1)]
        public string Instruks_URL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Undercategory> Undercategory { get; set; }
    }
}
