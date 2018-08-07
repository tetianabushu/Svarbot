using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class SvarbotBL
    {
        SvarbotDAL dal;
        AdminBL adminbl;

        public SvarbotBL() {
            dal = new SvarbotDAL();
            adminbl = new AdminBL();
        }

        public string GetLongInstructionTitle(int id) {
            return dal.GetLongInstructionTitle(id);
        }

        public string GetLongInstructionText(int id) {
            return dal.getLongInstructionText(id);
        }


        //metode for å søke i AllLayersForSearchObjectDTO object for søkekombinasjon som kommer inn 
        //fra søkefeltet
        //public List<KategoriDTO> GetSearchResult(string searchIn)
        //{
        //    List<AllLayersForSearchObjectDTO> liste = dal.GetAllItemsForSearch();
        //    List<KategoriDTO> mainCategoryList = new List<KategoriDTO>();
           
        //    foreach (var i in liste)
        //    {                
        //        if (i.ParentCatName.ToLower().Contains(searchIn.Trim().ToLower()) )
        //        {
        //            KategoriDTO mainCat = new KategoriDTO();
        //            mainCat.id = i.ParentCategoryId;
        //            mainCat.name = i.ParentCatName;
        //            //if(!mainCategoryList.Contains(mainCat.id))
        //            {
        //                mainCategoryList.Add(mainCat);
        //            }
                    
        //        }
                
        //    }
        //    return mainCategoryList;
        //}
        //Metode brukes ikke lenger Tetiana
        //public List<KategoriDTO> GetCategoriesFromDb()
        //{
        //    return dal.GetCategoriesFromDb();
        //}

        //Tetiana
        public List<CategoryTypeDTO> GetTypes()
        {
            return dal.GetTypes();
        }
                        
        public MainCategoryDetailsDTO GetAllUndercatFromDb(int mainCategoryId, string searchText, string username)
        {
            var underCategories = dal.GetAllUndercatFromDb(mainCategoryId, searchText, username);
            var category = dal.GetCategoryById(mainCategoryId);
            return new MainCategoryDetailsDTO
            {
                Id = category.id,
                Name = category.name,
                IsLoggedIn = username == null ? false:true,
                SubCategories = underCategories
            };
        }

        //Tetiana henter subcategorydetails objekt, når man klikker på underkategoy i svarbot
        public SubcategoryDetailsDTO GetUndercategoryDetails(int underCategoryId, string username)
        {
            return dal.GetSubCategoryById(underCategoryId, username);
        }

        public InstruksVeiledningDTO GetInstruksVeiledning(int id)
        {
            return dal.GetInstruksVeiledning(id);
        }

        public KategoriDTO GetCategoryById(int categoryId)
        {
            return dal.GetCategoryById(categoryId);
        }

        //Johan Sakshaug
        //Registers a user in the database. Returns true if successful, false if not
        public bool registerUser(UserDTO inUser) {
            return dal.RegistrerUser(inUser);
        }

        //Johan Sakshaug
        //Returns true if user with the given username and password exists in the database, returns false if not
        public bool userInDb(UserDTO inUser) {
            return dal.Bruker_i_DB(inUser);
        }

        //Margrete Sander
        //brukt ingen sted
        //public SkjemaDTO GetSkjemaDTO(int Id)
        //{
        //    return dal.GetSkjemaDTO(Id);
        //}

        //Margrete Sander : her kommer funksjon for Superbuker
        //Knyttet til Authenticate
        //flytter til AdminBL
        //public bool SuperuserInDb(SuperuserDTO InSuperDTO)
        //{
        //    return dal.Superbruker_i_DB(InSuperDTO);
        //}

        //Johan Sakshaug
        public List<SubcategoryDTO> GetFavoriteSubcategories(string username) {
            return dal.GetFavoriteSubcategories(username);
        }

        public void ToggleFavorite(string username, int subcategoryId) {
            dal.ToggleFavorite(username, subcategoryId);
        }

        //Johan Sakshaug
        public List<SkjemaDTO> GetSubmittedCases(string username) {
            return dal.GetSubmittedCases(username);
        }
        //Tetiana
        //metode henter alle kategorier fra DAL, og basert på type returnerer top 5 kategorier
        //fra statistikk eller alle kategorier med søkeresultatet
        public List<KategoriDTO> GetMainCategories(int typeId, int? count, string searchText)
        {
            var allCategories = dal.GetMainCategories(typeId, searchText);

            if (count.HasValue)
            {
                var topCategories = dal.GetTopCategoriesForSvarbot(typeId, count.Value).ToList();
                var result = (from c in allCategories
                             join top in topCategories on c.id equals top.CategoryId
                             select c).ToList();
                return result;
            }

            return allCategories;
        }

        //Johan Sakshaug
        //returns true if case was submitted successfully, false if not
        public bool SubmitCase(SkjemaDTO inCase) {
            return dal.SubmitCase(inCase);
        }


        //Johan Sakshaug
        // Tetiana: Ikke brukt lenger
        //public List<string> GetFavoriteSubcategoriesArray(string username) {
        //    return dal.GetFavoriteSubcategoriesArray(username);
        //}

        //Johan Sakshaug
        // Tetiana Ikke brukt lenger
        //public bool IsFavorite(int subcategoryId, string username) {
        //    return dal.IsFavorite(subcategoryId, username);
        //}
    }
}
