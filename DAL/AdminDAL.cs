using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DBModels;
using System.Data.Entity;

namespace DAL
{
    public class AdminDAL
    {
        private SvarbotDbSys _dbContext;

        public AdminDAL(SvarbotDbSys context)
        {
            _dbContext = context;
        }

        //Tetiana lagrer i db antall klikk på kategorier
        public bool SaveClickCount(ClickCountDTO clickIn)
        {
            try
            {
                var clickedOn = new ClickCount
                {
                    Username = clickIn.Username,
                    CategoryId = clickIn.CategoryId,
                    DateClicked = clickIn.ClickDate,
                    IsMainCat = clickIn.IsMainCat
                };
                _dbContext.ClickCount.Add(clickedOn);
                _dbContext.SaveChanges();
                return true;
            }
            
            catch (Exception)
            {
                return false;
            }
            
        }
        
        //skal hente fra Superuser tabell da, flytter til AdminDAL
        //metode returnerer brukernavn til superuser for å sette den i session videre
        //Tetiana
        public SuperuserDTO Superbruker_i_DB(SuperuserDTO inSuperuser)
        {
            byte[] passwordDb = SvarbotDAL.MakeHash(inSuperuser.Password);            
            var superUser = _dbContext.Superuser.FirstOrDefault(b => b.Username.ToLower() == inSuperuser.Username);
            var foundSuperuser = superUser.Password.SequenceEqual(passwordDb) ? superUser : null;
            if (foundSuperuser == null)
            {
                //RegistrerSuperUser(inSuperuser);
                return null;
            }
            else
            {
                SuperuserDTO verifyUserName = new SuperuserDTO();
                verifyUserName.Username = foundSuperuser.Username;
                return verifyUserName;
            }
        }
        //Tetiana
        // henter hoved kategorier som er mest klikket på fra database
        public List<CategoryStatDTO> GetCategoriesWithCount(DateTime from, DateTime til, int isMainCategory, int typeId)
        {
            try
            {                  
                var listFromDb = _dbContext.ClickCount
                        .Where(x => 
                                DbFunctions.TruncateTime(x.DateClicked) >= from.Date &&
                                DbFunctions.TruncateTime(x.DateClicked) <= til.Date &&
                                x.IsMainCat == isMainCategory);

                if (isMainCategory == 1)
                {
                    var resultToReturn = from x in listFromDb
                                            join cat in _dbContext.Categories
                                            on x.CategoryId equals cat.Id
                                            where cat.CategoryType.Id == typeId
                                            group x by x.CategoryId into gruppe
                                            select new CategoryStatDTO
                                            {
                                                CategoryId = gruppe.Key,
                                                Count = gruppe.Count()
                                            };

                    return resultToReturn.ToList();
                } else
                {
                    var resultToReturn = from x in listFromDb
                                            join subcat in _dbContext.Undercategory
                                            on x.CategoryId equals subcat.Id
                                            where subcat.Categories.CategoryType.Id == typeId
                                            group x by x.CategoryId into gruppe
                                            select new CategoryStatDTO
                                            {
                                                CategoryId = gruppe.Key,
                                                Count = gruppe.Count()
                                            };

                    return resultToReturn.ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        //Tetiana 
        //metode returnerer kategorier som en bruker klikket på mest
        public List<CategoryStatDTO> GetCategoriesClickedPerUser(string username)
        {

            try
            {
                var listOfCatPerUserQuery = from log in _dbContext.ClickCount
                                            join cat in _dbContext.Categories
                                                 on log.CategoryId equals cat.Id
                                            where log.Username == username && log.IsMainCat == 1
                                            group log by new { cat.Id, cat.Category_name } into g
                                            select new CategoryStatDTO
                                            {
                                                CategoryId = g.Key.Id,
                                                CategoryName = g.Key.Category_name,
                                                Count = g.Count()
                                            };
                var listOfCatPerUser = listOfCatPerUserQuery.Take(5).ToList();
                return listOfCatPerUser;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //Tetiana metode for å returnere saker. Enten alle saker, eller saker for et brukernavn
        public List<CaseDetailsDTO> GetCaseDetails(string username)
        {
            try
            {                   
                var caseList = new List<CaseDetailsDTO>();
                if (username == null)
                {
                    var caseListfromDb = _dbContext.Form.ToList();

                    foreach (var c in caseListfromDb)
                    {
                        var caseDetails = new CaseDetailsDTO
                        {
                            CaseId = c.Id,
                            UserName = c.Navn,
                            CaseDetails = c.Beskrivelse,
                            DateCreated = c.Date,
                            CategoryId = c.CategoryId,
                            CategoryName = c.Categories.Category_name,
                        };
                        caseList.Add(caseDetails);
                    }

                }else{
                    var caseListfromDb = _dbContext.Form.Where(x=>x.Navn== username).ToList();
                    foreach (var c in caseListfromDb)
                    {
                        var caseDetailsForUser = new CaseDetailsDTO
                        {
                            CaseId = c.Id,
                            UserName = c.Navn,
                            CaseDetails = c.Beskrivelse,
                            DateCreated = c.Date,
                        };
                        caseList.Add(caseDetailsForUser);
                    }
                }
                return caseList;
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Tetiana returnerer kategorier med navn
        public List<CategoryStatDTO> GetCategoryWithNames(List<CategoryStatDTO> gruppe, int isMainCategory)
        {            
            try {
                foreach (var c in gruppe)
                {
                    if (isMainCategory == 1)
                    {
                        var category = _dbContext.Categories.Where(x => x.Id == c.CategoryId).First();
                        c.CategoryName = category.Category_name;
                    }
                    else {
                        var subCategory = _dbContext.Undercategory.Where(x => x.Id == c.CategoryId).First();
                        c.CategoryName = subCategory.Undercategory_name;
                    }
                    
                }
                return gruppe;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //Tetiana
        //henter antall brukere med saker per bruker
        public List<UserAndCaseCountDTO> GetAllUsernamesCasesList()
        {
            try
            {
                var listOfCasesperUserQuery = from user in _dbContext.Accounts
                                            join sak in _dbContext.Form
                                            on user.navn equals sak.Navn
                                            group user by user.navn into g
                                            select new UserAndCaseCountDTO()
                                            {
                                                UserName = g.Key,
                                                CaseCount = g.Count()
                                            };
                var listOfCasesPerUser = listOfCasesperUserQuery.ToList();
                return listOfCasesPerUser;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        //Johan
        //get categories
        public List<KategoriDTO> GetCategories()
        {
            try {
                return _dbContext.Categories.Where(x=>x.Deleted==0).Select(c => new KategoriDTO()
                {
                    id = c.Id,
                    name = c.Category_name,
                    Underkategorier = c.Undercategory.Select(uc => new SubcategoryDTO()
                    {
                        Id = uc.Id,
                        Name = uc.Undercategory_name,
                        InstruksVeiledningId = uc.Instruks_Veiledning_Id,
                        Instructions = uc.Instruks_Veiledning.Inskruks_beskrivelse,
                        LongInstructionsID = uc.Langinstruks_id.HasValue ? uc.Langinstruks_id.Value : -1,
                        Category_Id = uc.Category_Id
                    }).ToList()
                }).ToList();
            }
            catch (Exception e)
            {

                throw;
            }
        }
        //Tetiana
        //returnerer liste av kategorier som har minst en sak opprettet med den kategori
        public List<CategoryWithCasesDTO> GetCategoriesWithCases()
        {
            try
            {                
                var listOfCategoriesWithCases = from category in _dbContext.Categories
                                                join sak in _dbContext.Form
                                                on category.Id equals sak.CategoryId
                                                group category by new
                                                {
                                                    category.Id,
                                                    category.Category_name
                                                } into gruppe
                                                select new CategoryWithCasesDTO()
                                                {
                                                    CategoryId = gruppe.Key.Id,
                                                    CategoryName = gruppe.Key.Category_name,
                                                    CasesPerCategory = gruppe.Count()
                                                };
                return listOfCategoriesWithCases.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }  
       

        //Johan Sakshaug
        public List<SubcategoryDTO> GetSubcategories(int id)
        {
            try {
                return _dbContext.Undercategory.Where(c => c.Category_Id == id)
                    .Select(c => new SubcategoryDTO()
                    {
                        Id = c.Id,
                        Name = c.Undercategory_name,
                        InstruksVeiledningId = c.Instruks_Veiledning_Id,
                        Instructions = c.Instruks_Veiledning.Inskruks_beskrivelse,
                        LongInstructionsID = c.Langinstruks_id.HasValue ? c.Langinstruks_id.Value : -1,
                        Category_Id = c.Category_Id
                    }).ToList();
            }
            catch(Exception e)
            {
                throw;
            }
        }
        
        //Johan
        //delete category
        public bool DeleteCategory(int id)
        {
            try {
                var c = _dbContext.Categories.FirstOrDefault(cat => cat.Id == id);
                if (c != null)
                {
                    c.Deleted=1;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        

        //Johan
        //Tetiana : forbedre og forenkle metode
        //add category
        public bool AddCategory(CategorySubmitDTO c)
        {
            try {
                //if category exists return false
                var cat = _dbContext.Categories.FirstOrDefault(x =>x.Category_name== c.CategoryName && x.Deleted==0);
                if (cat != null) return false;
                var category = new Categories();
                
                category.Category_type_Id = c.CategoryType;
                category.Category_name = c.CategoryName;

                var undercatTodb = new Undercategory()
                {
                    Undercategory_name = c.SubcategoryName
                };
                category.Undercategory.Add(undercatTodb);

                var instruks = new Instruks_Veiledning()
                {
                    Inskruks_beskrivelse = c.Instruction
                };
                undercatTodb.Instruks_Veiledning = instruks;
                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        //Johan
        //add subcategory
        public void AddSubcategory(int categoryId, string name, string instruction)
        {            
            try {
                var sub = new Undercategory()
                {
                    Category_Id = categoryId,
                    Undercategory_name = name,
                };
                sub.Instruks_Veiledning = new Instruks_Veiledning
                {
                    Inskruks_beskrivelse = instruction
                };
                _dbContext.Undercategory.Add(sub);
                _dbContext.SaveChanges();
            } catch(Exception e)
            {
                throw;
            }
        }
        
        //Johan
        //Tetiana
        //delete subcategory
        public void DeleteSubcategory(int subId)
        {
            try
            {
                var sub = _dbContext.Undercategory.SingleOrDefault(s => s.Id == subId);
                if (sub != null)
                {
                    //remove subcategory                    
                    if (sub.Instruks_Veiledning != null)
                    {
                        _dbContext.Instruks_Veiledning.Remove(sub.Instruks_Veiledning);
                    }
                    _dbContext.Undercategory.Remove(sub);
                    _dbContext.SaveChanges();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }        

        //Johan Sakshaug
        // Redigert, fjernet unødvendige db kall Tetiana
        public void UpdateInstruction(int subId, string instruction)
        {
            try
            {
                var subCategory = _dbContext.Undercategory.FirstOrDefault(x => x.Id == subId);
                if (subCategory != null && subCategory.Instruks_Veiledning != null)
                {
                    subCategory.Instruks_Veiledning.Inskruks_beskrivelse = instruction;
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Johan Sakshaug
        //la til deleted sjekk Tetiana
        public string GetCategoryName(int id)
        {
            try
            {
                return _dbContext.Categories.Single(c => c.Id == id && c.Deleted == 0).Category_name;
            }
            catch (Exception ex)
            {
                return null;
            }            
        }
    }
}
