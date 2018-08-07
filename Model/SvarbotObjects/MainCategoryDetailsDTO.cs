using System.Collections.Generic;

namespace Model
{
    //Tetiana
    //klasse objekt returnerer en hoved kategori med detaljer av underkategorier
    //med mindre data, dvs uten unstruks og ale annet, for å ikke hente unødvendig data flere ganger
    public class MainCategoryDetailsDTO
    {
        public MainCategoryDetailsDTO()
        {
            SubCategories = new List<SubcategoryListItemDTO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsLoggedIn { get; set; }

        public List<SubcategoryListItemDTO> SubCategories { get; set; }
    }
}
