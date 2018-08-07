using DAL;
using DAL.DBModels;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AdminBL
    {
        AdminDAL adminDAL;

        public AdminBL()
        {
            adminDAL = new AdminDAL(new SvarbotDbSys());
        }
        public SuperuserDTO SuperuserInDb(SuperuserDTO InSuperDTO)
        {
            return adminDAL.Superbruker_i_DB(InSuperDTO);
        }
		
        //Tetiana 
        //Metode lagrer klikk i database
        public void SaveClickCount(int categoryId, int isMainCat, string username)
        {

            var categoryClicked = new ClickCountDTO
            {
                CategoryId = categoryId,
                Username = username,
                ClickDate = DateTime.Now,
                IsMainCat = isMainCat
            };
            adminDAL.SaveClickCount(categoryClicked);
        }

        //Tetiana 
        //metode returnerer top 5 mest brukte/ klikket på kategorier
        public List<CategoryStatDTO> GetTopHovedCat(DateTime from, DateTime til, int isMainCategory, int typeId)
        {
            var gruppe = adminDAL.GetCategoriesWithCount(from, til, isMainCategory, typeId);
            gruppe = gruppe.OrderByDescending(x => x.Count).ToList();
            var result = adminDAL.GetCategoryWithNames(gruppe, isMainCategory).Take(5).ToList();
            return result;
        }

        //Tetiana
        //metode returnerer kategorier som en bruker klikket på mest
        public List<CategoryStatDTO> GetCategoriesClickedPerUser(string username)
        {
            return adminDAL.GetCategoriesClickedPerUser(username);
        }
        //Johan
        public List<KategoriDTO> GetCategories()
        {
        return adminDAL.GetCategories();
        }

        //Johan Sakshaug
        public List<SubcategoryDTO> GetSubcategories(int id) {
            return adminDAL.GetSubcategories(id);
        }

        //Johan 
        //change category name
        // brukes ikke
        //public bool RenameCategoryById(int id, string newName) {
        //    return adminDAL.RenameCategoryById(id, newName);
        //}
        //Johan
        //brukes ikke
        //public bool RenameCategoryByName(string oldName, string newName) {
        //    return adminDAL.RenameCategoryByName(oldName, newName);
        //}

        //Johan
        //delete category
        public bool DeleteCategory(int categoryId)
        {
            return adminDAL.DeleteCategory(categoryId);
        }

        //Johan
        //add category
        public bool AddCategory(CategorySubmitDTO c)
        {
            return adminDAL.AddCategory(c);
        }

        //Johan Sakshaug
        public string GetCategoryName(int id)
        {
            return adminDAL.GetCategoryName(id);
        }
        //Johan
        //add subcategory with instruction
        //ryddet Tetiana
        public void AddSubcategory(int categoryId, SubcategorySubmitDTO s)
        {
            adminDAL.AddSubcategory(categoryId, s.Name, s.Instruction);
        }

        //Johan Sakshaug
        //ryddet Tetiana
        public void DeleteSubcategory(int subId) {
            adminDAL.DeleteSubcategory(subId);
        }

        //Johan Sakshaug
        //tetiana redigert, fjernet unødvendige db kall
        public void UpdateInstruction(int subId, string instruction)
        {
            adminDAL.UpdateInstruction(subId, instruction);
        }
        //Tetiana
        //metode for å hente brukernavn og antall saker per bruker
        public List<UserAndCaseCountDTO> GetAllUsernamesCasesList()
        { 
            var userandcaselist = adminDAL.GetAllUsernamesCasesList().OrderByDescending(x=>x.CaseCount).ToList();

            return userandcaselist;
        }

        //Tetiana 
        //metode for å returnere saker per bruker eller alle saker, avgengig av parameter
        public List<CaseDetailsDTO> GetCaseDetails(string username)
        {
            var listOverCases = adminDAL.GetCaseDetails(username);
            return listOverCases;
        }
        //Tetiana
        //metode returnerer categori med saker
        public List<CategoryWithCasesDTO> GetCategoriesWithCases()
        {
            return adminDAL.GetCategoriesWithCases();
        }
    }
}

