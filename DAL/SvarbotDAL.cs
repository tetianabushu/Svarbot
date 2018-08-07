using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Web.Script.Serialization;
using DAL.DBModels;
using System.Data.Entity.Infrastructure;

namespace DAL
{
    public class SvarbotDAL {
        //Johan Sakshaug
        //takes the id of a long instruction as input
        //returns the title associated with the long instruction(the title is just the name of the subcategory it belongs to)
        public string GetLongInstructionTitle(int id)
        {
            using (var db = new SvarbotDbSys())
            {
                var subCategory = db.Undercategory.SingleOrDefault(u => u.Langinstruks_id == id);
                if (subCategory == null) return "Instruksjonstittel ikke funnet";
                else return subCategory.Undercategory_name;
            }
        }

        //Johan Sakshaug
        //takes the id of a long instruction as input
        //returns the text describing the long instruction
        public string getLongInstructionText(int id)
        {
            using (var db = new SvarbotDbSys())
            {
                var longInstruction = db.Langinstruks.SingleOrDefault(l => l.langinstruks_id == id);
                if (longInstruction == null) return "Instruksjonstekst ikke funnet";
                else return longInstruction.tekst;
            }
        }
        //Tetiana
        //hente hoved kategori by Id
        public KategoriDTO GetCategoryById(int categoryId) {
            try {
                using (var db = new SvarbotDbSys()) {
                    
                    var categoryFromDb = db.Categories.Where(x => x.Id == categoryId && x.Deleted==0).SingleOrDefault();
                    if(categoryFromDb == null)
                    {
                        throw new Exception("Kategori har status slettet.");
                    }
                    KategoriDTO catToReturn = new KategoriDTO();
                    catToReturn.id = categoryFromDb.Id;
                    catToReturn.name = categoryFromDb.Category_name;
                    return catToReturn;
                }
            }
            catch (Exception) {

                throw;
            }
        }
        


        //Tetiana 
        //metode henter to typer fra db pc eller service
        public List<CategoryTypeDTO> GetTypes()
        {
            try
            {
                using (var db = new SvarbotDbSys())
                {                    
                    var types = db.CategoryType.ToList();
                    var list = new List<CategoryTypeDTO>();
                    foreach (var item in types)
                    {
                        list.Add(new CategoryTypeDTO
                        {
                            Id = item.Id,
                            Name = item.TypeName
                        });
                    }
                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        //Tetiana
        //metode henter hoved kategorier fra db basert på type
        //hvis det er en search text, søker i database, hvis ikke, gjør det som er over
        public List<KategoriDTO> GetMainCategories(int typeId, string searchText)
        {
            try
            {
                using (var db = new SvarbotDbSys())
                {
                    IQueryable<Categories> query;
                    if(string.IsNullOrEmpty(searchText))
                    {
                        query = db.Categories.Where(x => x.Category_type_Id == typeId && x.Deleted == 0);                        
                    }
                    else
                    {
                        query = db.Categories
                        .Where(x => x.Category_type_Id == typeId && x.Deleted == 0 &&
                                        (x.Category_name.Contains(searchText.ToLower()) ||
                                         x.Undercategory.Any(under =>
                                                under.Undercategory_name.Contains(searchText.ToLower()) ||
                                                under.Instruks_Veiledning.Inskruks_beskrivelse.Contains(searchText.ToLower()))));
                    }
                    var items = query.ToList();

                    var list = new List<KategoriDTO>();

                    foreach (var item in items)
                    {
                        var category = new KategoriDTO()
                        {
                            id = item.Id,
                            name = item.Category_name
                        };
                        list.Add(category);
                    }
                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Tetiana
        // Henter top 5 categories for typeId
        public List<CategoryStatDTO> GetTopCategoriesForSvarbot(int typeId, int count)
        {
            try
            {
                using (var db = new SvarbotDbSys())
                {
                    var query = from click in db.ClickCount
                                join cat in db.Categories on click.CategoryId equals cat.Id
                                where click.IsMainCat == 1 &&
                                        cat.CategoryType.Id == typeId &&
                                        cat.Deleted == 0
                                group click by click.CategoryId into gr
                                select new CategoryStatDTO
                                {
                                    CategoryId = gr.Key,
                                    Count = gr.Count()
                                };
                    var result = query.OrderByDescending(x => x.Count).Take(count).ToList();
                    return result;
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        //Tetiana henter subcategorydetails objekt, når man klikker på underkategoy i svarbot
        public SubcategoryDetailsDTO GetSubCategoryById(int underCategoryId, string username)
        {
            try
            {
                using (var db = new SvarbotDbSys())
                {
                    var undercategory = db.Undercategory.FirstOrDefault(x => x.Id == underCategoryId);
                    if (undercategory == null) return null;    
                    var result = new SubcategoryDetailsDTO()
                    {
                        Id = undercategory.Id,
                        Name = undercategory.Undercategory_name,
                        MainCatId = undercategory.Categories.Id,
                        MainCatName = undercategory.Categories.Category_name,
                        Instructions = undercategory.Instruks_Veiledning.Inskruks_beskrivelse,
                        IsLoggedIn = username == null ? false: true
                    };
                    
                    if (undercategory.Favorites != null && undercategory.Favorites.Count > 0 &&
                            username != null && undercategory.Favorites.First().Brukernavn == username)
                    {
                        result.IsFavorite = true;
                    }

                    return result;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
                
        //Tetiana
        //get all subcategories for a specific maincategory id
        public List<SubcategoryListItemDTO> GetAllUndercatFromDb(int mainCategoryId, string searchText, string username)
        {
            try
            {
                using (var db = new SvarbotDbSys())
                {
                    IQueryable<Undercategory> query;

                    if (string.IsNullOrEmpty(searchText))
                        query = db.Undercategory.Where(x => x.Category_Id == mainCategoryId);
                    else
                    {
                        query = db.Undercategory.Where(x =>
                                x.Category_Id == mainCategoryId &&
                                    (x.Undercategory_name.Contains(searchText.ToLower()) ||
                                    x.Instruks_Veiledning.Inskruks_beskrivelse.Contains(searchText.ToLower())));

                    }
                    
                    var queryMedFavorites = from cat in query
                                            select new { underCat = cat, favorites = cat.Favorites.Where(f => f.Brukernavn == username) };

                    var subCategoriesfromDb = queryMedFavorites.ToList();

                    var undercatToReturn = new List<SubcategoryListItemDTO>();
                    foreach (var u in subCategoriesfromDb)
                    {
                        var underCategory = new SubcategoryListItemDTO
                        {
                            Id = u.underCat.Id,
                            Name = u.underCat.Undercategory_name                            
                        };

                        if (u.favorites != null && u.favorites.Count() > 0 && username != null)
                        {
                            underCategory.IsFavorite = true;
                        }
                        undercatToReturn.Add(underCategory);
                    }
                    
                    return undercatToReturn;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        //Johan Sakshaug
        //gets an instruction by given id
        //Tetiana
        //la til exception handling
        //opprettet Model, endret til at den returnerer hele objektet for å bruke videre
        public InstruksVeiledningDTO GetInstruksVeiledning(int id)
        {
            try {
                using (var db = new SvarbotDbSys())
                {
                    var instruks = db.Instruks_Veiledning.FirstOrDefault(i => i.Id == id);
                    InstruksVeiledningDTO instruksToReturn = new InstruksVeiledningDTO();
                    if (instruks == null)
                    {
                        instruksToReturn.InstruksDescription = "Kunne ikke finne instruks";
                        return instruksToReturn;
                    }
                    instruksToReturn.Id = instruks.Id;
                    instruksToReturn.InstruksDescription = instruks.Inskruks_beskrivelse;

                    return instruksToReturn;
                }
            }
            catch (Exception) {

                throw;
            }
        }

        //Johan Sakshaug
        //Hashes a password
        //gjør public siden bruker i admindal
        public static byte[] MakeHash(string innPassord)
        {
            byte[] innData, utData;
            var algoritme = System.Security.Cryptography.SHA256.Create();
            innData = System.Text.Encoding.ASCII.GetBytes(innPassord);
            utData = algoritme.ComputeHash(innData);
            return utData;
        }

        //Johan Sakshaug
        //Registers a user in the database. Returns true if successful, false if not
        public bool RegistrerUser(UserDTO inUser)
        {
            using (var db = new SvarbotDbSys())
            {
                try
                {
                    var newUser = new Accounts();
                    newUser.navn = inUser.username;
                    byte[] passwordDb = MakeHash(inUser.password);
                    newUser.passord = passwordDb;

                    db.Accounts.Add(newUser);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        //Johan Sakshaug
        //Returns true if user with the given username and password exists in the database, returns false if not
        public bool Bruker_i_DB(UserDTO inUser)
        {
            using (var db = new SvarbotDbSys())
            {
                byte[] passwordDb = MakeHash(inUser.password);
                var foundUser = db.Accounts.FirstOrDefault(b => b.passord == passwordDb && b.navn == inUser.username);
                if (foundUser == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
                

        //Johan Sakshaug
        //toggle a subcategory as favorite
        public void ToggleFavorite(string username, int subCategoryId)
        {
            using (var db = new SvarbotDbSys())
            {
                var favorite = db.Favorites.SingleOrDefault(f => f.Brukernavn == username && f.UndercategoryId == subCategoryId);
                if (favorite == null)
                {
                    Favorites favoriteToAdd = new Favorites
                    {
                        Brukernavn = username,
                        UndercategoryId = subCategoryId
                    };
                    db.Favorites.Add(favoriteToAdd);
                }
                else
                {
                    db.Favorites.Remove(favorite);
                }
                db.SaveChanges();
            }
        }

        //Johan Sakshaug henter liste av favoritter for et brukernavn
        public List<SubcategoryDTO> GetFavoriteSubcategories(string username)
        {
            using (var db = new SvarbotDbSys())
            {
                var subs = db.Favorites.Where(f => f.Brukernavn == username &&
                    f.Undercategory != null && f.Undercategory.Categories != null &&
                    f.Undercategory.Categories.Deleted == 0)
                    .Select(f => f.Undercategory)
                    .Select(u => new SubcategoryDTO()
                    {
                        Id = u.Id,
                        Name = u.Undercategory_name,
                        InstruksVeiledningId = u.Instruks_Veiledning_Id,
                        Instructions = u.Instruks_Veiledning.Inskruks_beskrivelse,
                        LongInstructionsID = u.Langinstruks_id.HasValue ? u.Langinstruks_id.Value : 0,
                        Category_Id = u.Category_Id
                    });

                return subs.ToList();
            }
        }

        //Johan Sakshaug
        //returns submitted cases for a given username
        public List<SkjemaDTO> GetSubmittedCases(string username)
        {
            using(var db = new SvarbotDbSys()) {
                return db.Form.Where(f => f.Navn == username)
                    .Select(f => new SkjemaDTO() {
                        Id = f.Id,
                        Username = f.Navn,
                        SBnumber = f.SBNummer,
                        Location = f.RegionBygg,
                        Message = f.Beskrivelse,
                        UserNr = f.AntallBerort
                    }).ToList();
            }
        }
        
        //Johan Sakshaug
        //returns true if case was submitted successfully, false if not
        //redo Tetiana
        public bool SubmitCase(SkjemaDTO inCase)
        {
            try
            {
                using (var db = new SvarbotDbSys())
                {
                    var f = new Form()
                    {
                        Id = inCase.Id,
                        Navn = inCase.Username,
                        SBNummer = inCase.SBnumber,
                        RegionBygg = inCase.Location,
                        Beskrivelse = inCase.Message,
                        AntallBerort = inCase.UserNr,
                        CategoryId = inCase.CategoryId,
                        Date = DateTime.Now
                    };
                    db.Form.Add(f);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {

                return false;
            }
            
        }
    }
}