using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL;
using DAL.DBModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Moq;

namespace SvarbotUnitTests.DALTests
{
    //Tetiana
    [TestClass]
    public class AdminDalTest
    {
        //Tetiana
        [TestMethod]
        public void GetCategoryName_gets_name_of_category()
        {
            var categories = new List<Categories>
            {
                new Categories { Id = 123, Category_name = "test category 1" },
                new Categories { Id = 345, Category_name = "test category 2" }
            }.AsQueryable();

            var catMock = new Mock<DbSet<Categories>>();
            catMock.As<IQueryable<Categories>>().Setup(m => m.Provider).Returns(categories.Provider);
            catMock.As<IQueryable<Categories>>().Setup(m => m.Expression).Returns(categories.Expression);
            catMock.As<IQueryable<Categories>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            catMock.As<IQueryable<Categories>>().Setup(m => m.GetEnumerator()).Returns(() => categories.GetEnumerator());
            
            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Categories).Returns(catMock.Object);

            var dalObject = new AdminDAL(dbMock.Object);

            var navn = dalObject.GetCategoryName(123);
            Assert.AreEqual(navn, "test category 1");

            var navn2 = dalObject.GetCategoryName(565656);
            Assert.IsNull(navn2);
        }

        //Tetiana
        [TestMethod]
        public void UpdateInstruction_updates_instrutionText_of_undercategory()
        {
            var und1 = new Undercategory
            {
                Id = 111,
                Instruks_Veiledning = new Instruks_Veiledning
                {
                    Inskruks_beskrivelse = "Text original 1"
                }
            };

            var und2 = new Undercategory
            {
                Id = 222,
                Instruks_Veiledning = new Instruks_Veiledning
                {
                    Inskruks_beskrivelse = "Text original 2"
                }
            };
            var underCategories = new List<Undercategory> { und1, und2 }.AsQueryable();

            var catMock = new Mock<DbSet<Undercategory>>();
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.Provider).Returns(underCategories.Provider);
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.Expression).Returns(underCategories.Expression);
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.ElementType).Returns(underCategories.ElementType);
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.GetEnumerator()).Returns(() => underCategories.GetEnumerator());

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Undercategory).Returns(catMock.Object);

            var dalObject = new AdminDAL(dbMock.Object);
            dalObject.UpdateInstruction(222, "New Text 2");

            Assert.AreEqual(und2.Instruks_Veiledning.Inskruks_beskrivelse, "New Text 2");

        }
        //Tetiana
        [TestMethod]
        public void DeleteSubcategory_deletes_subcategory()
        {
            var und1 = new Undercategory
            {
                Id = 22,
                Instruks_Veiledning = new Instruks_Veiledning
                {
                    Id = 15,
                    Inskruks_beskrivelse = "Text original 3"
                }
               
            };

            var und2 = new Undercategory
            {
                Id = 33,
                Instruks_Veiledning = new Instruks_Veiledning
                {
                    Id = 16,
                    Inskruks_beskrivelse = "Text original 5"
                    
                }

            };
            var underCategories = new List<Undercategory> { und1, und2 }.AsQueryable();

            var catMock = new Mock<DbSet<Undercategory>>();
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.Provider).Returns(underCategories.Provider);
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.Expression).Returns(underCategories.Expression);
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.ElementType).Returns(underCategories.ElementType);
            catMock.As<IQueryable<Undercategory>>().Setup(m => m.GetEnumerator()).Returns(() => underCategories.GetEnumerator());

            var instruksMock = new Mock<DbSet<Instruks_Veiledning>>();
            
            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Undercategory).Returns(catMock.Object);
            dbMock.Setup(x => x.Instruks_Veiledning).Returns(instruksMock.Object);

            var dalObject = new AdminDAL(dbMock.Object);
            dalObject.DeleteSubcategory(22);

            catMock.Verify(x => x.Remove(It.Is<Undercategory>(u => u.Id == 22)), Times.Once);
            instruksMock.Verify(x => x.Remove(It.Is<Instruks_Veiledning>(i => i.Id == 15)), Times.Once);
        }
        //Tetiana
        [TestMethod]
        public void SaveClickCount_adds_new_clickcount_object()
        {
            var clickCountMock = new Mock<DbSet<ClickCount>>();
            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.ClickCount).Returns(clickCountMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var testDate = DateTime.Now;

            var result = dalObject.SaveClickCount(new ClickCountDTO { Username = "tebu", ClickDate = testDate });

            Assert.IsTrue(result);
            clickCountMock.Verify(x => x.Add(It.Is<ClickCount>(click => 
                                        click.Username == "tebu" &&
                                        click.DateClicked == testDate)), 
                            Times.Once);
            dbMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        //Tetiana
        [TestMethod]
        public void Superbruker_i_DB_returnerer_brukernavn_til_superbruker()
        {
            var dbusers = new List<Superuser> { new Superuser { Username = "testuser", Password = SvarbotDAL.MakeHash("1234") } }
                            .AsQueryable();

            var superUserMock = new Mock<DbSet<Superuser>>();
            superUserMock.As<IQueryable<Superuser>>().Setup(m => m.Provider).Returns(dbusers.Provider);
            superUserMock.As<IQueryable<Superuser>>().Setup(m => m.Expression).Returns(dbusers.Expression);
            superUserMock.As<IQueryable<Superuser>>().Setup(m => m.ElementType).Returns(dbusers.ElementType);
            superUserMock.As<IQueryable<Superuser>>().Setup(m => m.GetEnumerator()).Returns(() => dbusers.GetEnumerator());

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Superuser).Returns(superUserMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.Superbruker_i_DB(new SuperuserDTO { Username = "testuser", Password = "1234" });
            Assert.AreEqual("testuser", result.Username);

            var result1 = dalObject.Superbruker_i_DB(new SuperuserDTO { Username = "testuser", Password = "feilpassord" });
            Assert.IsNull(result1);
        }

        //Tetiana
        [TestMethod]
        public void GetCategoriesClickedPerUser_returner_bruker_stats_for_Main_category()
        {
            var clicks = new List<ClickCount> {
                new ClickCount { Username = "testuser", Id = 1, IsMainCat = 1, CategoryId = 123 },
                new ClickCount { Username = "testuser", Id = 2, IsMainCat = 1, CategoryId = 4455 },
                new ClickCount { Username = "testuser", Id = 3, IsMainCat = 1, CategoryId = 4455 },
                new ClickCount { Username = "testuser", Id = 4, IsMainCat = 0, CategoryId = 333 }}.AsQueryable();

            var clicksMock = new Mock<DbSet<ClickCount>>();
            SetupMockLinq(clicks, clicksMock);

            var categories = new List<Categories> {
                new Categories { Id = 123, CategoryType = new CategoryType{ Id = 1} },
                new Categories { Id = 4455, CategoryType = new CategoryType{ Id = 2} },
                new Categories { Id = 333, CategoryType = new CategoryType{ Id = 1} } }.AsQueryable();
            var categoriesMock = new Mock<DbSet<Categories>>();
            SetupMockLinq(categories, categoriesMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.ClickCount).Returns(clicksMock.Object);
            dbMock.Setup(x => x.Categories).Returns(categoriesMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.GetCategoriesClickedPerUser("testuser");
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result.Single(x => x.CategoryId == 123).Count);
            Assert.AreEqual(2, result.Single(x => x.CategoryId == 4455).Count);
        }

        //Tetiana
        [TestMethod]
        public void GetCaseDetails_returns_all_cases_for_user_or_all_cases_for_all_users()
        {
            var forms = new List<Form> {
                new Form { Id = 1, Navn = "testuser1", Categories  = new Categories() },
                new Form { Id = 2, Navn = "testuser2", Categories  = new Categories() },
                new Form { Id = 3, Navn = "testuser1", Categories  = new Categories() }}.AsQueryable();

            var formsMock = new Mock<DbSet<Form>>();
            SetupMockLinq(forms, formsMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Form).Returns(formsMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.GetCaseDetails("testuser1");
            Assert.AreEqual(2, result.Count);

            result = dalObject.GetCaseDetails("testuser2");
            Assert.AreEqual(1, result.Count);

            result = dalObject.GetCaseDetails("testuser3");
            Assert.AreEqual(0, result.Count);

            result = dalObject.GetCaseDetails(null);
            Assert.AreEqual(3, result.Count);
        }

        //Tetiana
        [TestMethod]
        public void GetCategoryWithNames_returns_main_categories_with_names()
        {
            var categories = new List<Categories> {
                new Categories { Id = 11, Category_name = "cat1" },
                new Categories { Id = 22, Category_name = "cat2"},
                new Categories { Id = 111222, Category_name = "cat3"} }.AsQueryable();
            var categoriesMock = new Mock<DbSet<Categories>>();
            SetupMockLinq(categories, categoriesMock);
            
            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Categories).Returns(categoriesMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var param = new List<CategoryStatDTO>
            {
                new CategoryStatDTO{CategoryId = 11},
                new CategoryStatDTO{CategoryId = 111222}
            };

            var result = dalObject.GetCategoryWithNames(param, 1);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("cat1", result.Single(x => x.CategoryId == 11).CategoryName);
            Assert.AreEqual("cat3", result.Single(x => x.CategoryId == 111222).CategoryName);
        }

        //Tetiana
        [TestMethod]
        public void GetCategoryWithNames_returns_subCategories_with_names()
        {
            var subCats = new List<Undercategory> {
                new Undercategory { Id = 333, Undercategory_name = "sub1" },
                new Undercategory { Id = 444, Undercategory_name = "sub2" }}.AsQueryable();
            var subCatsMock = new Mock<DbSet<Undercategory>>();
            SetupMockLinq(subCats, subCatsMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Undercategory).Returns(subCatsMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var param = new List<CategoryStatDTO>
            {
                new CategoryStatDTO{CategoryId = 333},
                new CategoryStatDTO{CategoryId = 444}
            };

            var result = dalObject.GetCategoryWithNames(param, 0);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("sub1", result.Single(x => x.CategoryId == 333).CategoryName);
            Assert.AreEqual("sub2", result.Single(x => x.CategoryId == 444).CategoryName);
        }

        //Tetiana
        [TestMethod]
        public void GetAllUsernamesCasesList_gets_users_and_their_case_count()
        {
            var forms = new List<Form> {
                new Form { Id = 1, Navn = "testuser1"},
                new Form { Id = 2, Navn = "testuser2"},
                new Form { Id = 3, Navn = "testuser1"}}.AsQueryable();

            var formsMock = new Mock<DbSet<Form>>();
            SetupMockLinq(forms, formsMock);

            var accounts = new List<Accounts> {
                new Accounts { navn = "testuser1"},
                new Accounts { navn = "testuser2"},
                new Accounts { navn = "testuser3"}}.AsQueryable();

            var accountsMock = new Mock<DbSet<Accounts>>();
            SetupMockLinq(accounts, accountsMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Form).Returns(formsMock.Object);
            dbMock.Setup(x => x.Accounts).Returns(accountsMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.GetAllUsernamesCasesList();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result.Single(x => x.UserName == "testuser1").CaseCount);
            Assert.AreEqual(1, result.Single(x => x.UserName == "testuser2").CaseCount);
        }

        //Tetiana
        [TestMethod]
        public void GetCategories_returns_all_mainCategories_with_underCat()
        {
            var categories = new List<Categories> {
                new Categories { Id = 11, Category_name = "cat1",
                    Undercategory = new List<Undercategory>
                    {
                        new Undercategory { Id = 111, Undercategory_name = "sub111", Instruks_Veiledning = new Instruks_Veiledning(), Langinstruks = new Langinstruks() }
                    } },
                new Categories { Id = 22, Category_name = "cat2",
                    Undercategory = new List<Undercategory>
                    {
                        new Undercategory { Id = 222, Undercategory_name = "sub222", Instruks_Veiledning = new Instruks_Veiledning(), Langinstruks = new Langinstruks() },
                        new Undercategory { Id = 333, Undercategory_name = "sub333", Instruks_Veiledning = new Instruks_Veiledning(), Langinstruks = new Langinstruks() }
                    } } }.AsQueryable();

            var categoriesMock = new Mock<DbSet<Categories>>();
            SetupMockLinq(categories, categoriesMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Categories).Returns(categoriesMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.GetCategories();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result.Single(x => x.id == 11).Underkategorier.Count);
            Assert.AreEqual(2, result.Single(x => x.id == 22).Underkategorier.Count);
        }

        //Tetiana
        [TestMethod]
        public void AddCategory_adds_new_category()
        {
            var categories = new List<Categories> {
                new Categories { Id = 11, Category_name = "cat1" } }.AsQueryable();
            var catMock = new Mock<DbSet<Categories>>();
            SetupMockLinq(categories, catMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Categories).Returns(catMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.AddCategory(new CategorySubmitDTO {  CategoryName = "cat2", SubcategoryName = "sub2", Instruction = "intruks" });

            Assert.IsTrue(result);
            catMock.Verify(x => x.Add(It.Is<Categories>(c =>
                                        c.Category_name == "cat2" &&
                                        c.Undercategory.Single().Undercategory_name == "sub2" &&
                                        c.Undercategory.Single().Instruks_Veiledning.Inskruks_beskrivelse == "intruks")),
                            Times.Once);
            dbMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        //Tetiana
        [TestMethod]
        public void AddSubcategory_adds_sub_category()
        {
            var underCatMock = new Mock<DbSet<Undercategory>>();
            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Undercategory).Returns(underCatMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);
            
            dalObject.AddSubcategory(1, "underCat 123", "Under cat 123 instruks");

            underCatMock.Verify(x => x.Add(It.Is<Undercategory>(under =>
                                        under.Undercategory_name == "underCat 123" &&
                                        under.Instruks_Veiledning.Inskruks_beskrivelse == "Under cat 123 instruks")),
                            Times.Once);
            dbMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        //Tetiana
        [TestMethod]
        public void DeleteCategory_deletes_category()
        {
            var categories = new List<Categories> {
                new Categories { Id = 11, Category_name = "cat1" } }.AsQueryable();
            var catMock = new Mock<DbSet<Categories>>();
            SetupMockLinq(categories, catMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Categories).Returns(catMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.DeleteCategory(11);
            Assert.IsTrue(result);
            Assert.AreEqual(1, categories.Single(x => x.Id == 11).Deleted);
            dbMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        //Tetiana
        [TestMethod]
        public void GetCategoriesWithCases_get_categories_joined_with_cases()
        {
            var categories = new List<Categories> {
                new Categories { Id = 222, Category_name = "cat1" },
                new Categories { Id = 333, Category_name = "cat2" },
                new Categories { Id = 444, Category_name = "cat3" }}.AsQueryable();
            var catMock = new Mock<DbSet<Categories>>();
            SetupMockLinq(categories, catMock);

            var forms = new List<Form> {
                new Form { Id = 1, CategoryId = 333 },
                new Form { Id = 2, CategoryId = 444 },
                new Form { Id = 3, CategoryId = 333 }}.AsQueryable();

            var formsMock = new Mock<DbSet<Form>>();
            SetupMockLinq(forms, formsMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.Categories).Returns(catMock.Object);
            dbMock.Setup(x => x.Form).Returns(formsMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.GetCategoriesWithCases();
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result.Single(x => x.CategoryId == 333).CasesPerCategory);
            Assert.AreEqual(1, result.Single(x => x.CategoryId == 444).CasesPerCategory);
        }

        //Tetiana
        [TestMethod]
        public void GetSubcategories_returns_subCategories_of_1_hoved_category()
        {
            //var categories = new List<Categories> {
            //    new Categories { Id = 222, Category_name = "cat1" },
            //    new Categories { Id = 444, Category_name = "cat2" }}.AsQueryable();
            //var catMock = new Mock<DbSet<Categories>>();
            //SetupMockLinq(categories, catMock);

            var underCategories = new List<Undercategory> {
                new Undercategory { Id = 1, Undercategory_name = "sub1",  Category_Id = 222, Instruks_Veiledning = new Instruks_Veiledning(), Langinstruks = new Langinstruks() },
                new Undercategory { Id = 2, Undercategory_name = "sub2", Category_Id = 222, Instruks_Veiledning = new Instruks_Veiledning(), Langinstruks = new Langinstruks() },
                new Undercategory { Id = 3, Undercategory_name = "sub3", Category_Id = 444, Instruks_Veiledning = new Instruks_Veiledning(), Langinstruks = new Langinstruks() }}.AsQueryable();

            var underMock = new Mock<DbSet<Undercategory>>();
            SetupMockLinq(underCategories, underMock);

            var dbMock = new Mock<SvarbotDbSys>();            
            dbMock.Setup(x => x.Undercategory).Returns(underMock.Object);
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.GetSubcategories(222);
            Assert.AreEqual(2, result.Count);
            Assert.IsNotNull(result.Single(x => x.Id == 1 && x.Name == "sub1"));
            Assert.IsNotNull(result.Single(x => x.Id == 2 && x.Name == "sub2"));

            result = dalObject.GetSubcategories(444);
            Assert.AreEqual(1, result.Count);
            Assert.IsNotNull(result.Single(x => x.Id == 3 && x.Name == "sub3"));
        }

        //Tetiana
        [Ignore] // feiler pga DbFunctions.TruncateTime, veldig vanskelig å finne løsning på det.
        [TestMethod] 
        public void GetCategoriesWithCount_returns_Categories_with_count()
        {
            var date = new DateTime(2018, 2, 1);
            var clicks = new List<ClickCount> {
                new ClickCount { Username = "testuser", Id = 1, DateClicked = date, IsMainCat = 1, CategoryId = 11 },
                new ClickCount { Username = "testuser", Id = 2, DateClicked = date.AddDays(1), IsMainCat = 1, CategoryId = 22 },
                new ClickCount { Username = "testuser", Id = 3, DateClicked = date.AddDays(2), IsMainCat = 1, CategoryId = 22 },
                new ClickCount { Username = "testuser", Id = 4, DateClicked = date.AddDays(3), IsMainCat = 0, CategoryId = 333 }}.AsQueryable();

            var clicksMock = new Mock<DbSet<ClickCount>>();
            SetupMockLinq(clicks, clicksMock);

            var categories = new List<Categories> {
                new Categories { Id = 11, CategoryType = new CategoryType{ Id = 1} },
                new Categories { Id = 22, CategoryType = new CategoryType{ Id = 2} },
                new Categories { Id = 111222, CategoryType = new CategoryType{ Id = 1} } }.AsQueryable();
            var categoriesMock = new Mock<DbSet<Categories>>();
            SetupMockLinq(categories, categoriesMock);

            var subCats = new List<Undercategory> {
                new Undercategory { Id = 333, Categories = new Categories{ CategoryType = new CategoryType{ Id = 1} } },
                new Undercategory { Id = 444, Categories = new Categories{ CategoryType = new CategoryType{ Id = 1} } }}.AsQueryable();
            var subCatsMock = new Mock<DbSet<Undercategory>>();
            SetupMockLinq(subCats, subCatsMock);

            var dbMock = new Mock<SvarbotDbSys>();
            dbMock.Setup(x => x.ClickCount).Returns(clicksMock.Object);
            dbMock.Setup(x => x.Categories).Returns(categoriesMock.Object);
            dbMock.Setup(x => x.Undercategory).Returns(subCatsMock.Object);
            
            var dalObject = new AdminDAL(dbMock.Object);

            var result = dalObject.GetCategoriesWithCount(date, date.AddDays(10), 1, 1);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result.Single(x => x.CategoryId == 11).Count);
            Assert.AreEqual(2, result.Single(x => x.CategoryId == 22).Count);
        }

        //Tetiana
        private static void SetupMockLinq<T>(IQueryable<T> clicks, Mock<DbSet<T>> clicksMock) where T : class
        {
            clicksMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(clicks.Provider);
            clicksMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(clicks.Expression);
            clicksMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(clicks.ElementType);
            clicksMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => clicks.GetEnumerator());
        }
    }
}
