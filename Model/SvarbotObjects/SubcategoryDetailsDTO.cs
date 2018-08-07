namespace Model
{
    //tetiana
    //klasse objekt for å vise instruksjoner i svarbor vindu
    public class SubcategoryDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }

        public int MainCatId { get; set; }
        public string MainCatName { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
