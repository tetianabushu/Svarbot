using System.Collections.Generic;

namespace Model
{


    public class KategoriDTO
    {
        public KategoriDTO()
        {
            Underkategorier = new List<SubcategoryDTO>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public List<SubcategoryDTO> Underkategorier { get; set; }
    }
}
