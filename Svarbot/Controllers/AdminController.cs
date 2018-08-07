using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Svarbot.Controllers
{
   
    //Tetiana endrer Admin Controller, siden lagde AdminBL og AdminDAL 
    //for å separere fra Svarbot relatert logikk
    public class AdminController : Controller
    {
        private AdminBL adminBL;

        public AdminController()
        {
            adminBL = new AdminBL();
        }

        public AdminController(AdminBL adminBLL)
        {
            adminBL = adminBLL;
        }
        //Tetiana metode som får inn id av kategori som ble klikket på, 
        //om kat er hoved eller ikke og brukernavn
        public void SaveClickedCategory(int id, int isMainCat)
        {
            var Username = "";
            if (LoggedInBruker())
            {
                Username = (string)Session["LoggedInUsername"];
            }
            else { Username = "Guest"; }
            adminBL.SaveClickCount(id,isMainCat,Username);

        }
        //Tetiana metode for å hente top 5 basert på type og datoer
        public JsonResult GetTopHovedCat(string dateFrom, string dateTil, int isMain, int typeId)
        {
            var dateFromDateTime = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dateTilDateTime = DateTime.ParseExact(dateTil, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var listOfTopCat = adminBL.GetTopHovedCat(dateFromDateTime, dateTilDateTime, isMain, typeId);

            return Json(listOfTopCat, JsonRequestBehavior.AllowGet);

        }

        //Tetiana
        //metode returnerer liste av kategorier med saker opprettet
        public JsonResult GetCategoriesWithCases()
        {
           var listOfCatWithCases = adminBL.GetCategoriesWithCases();
            return Json(listOfCatWithCases, JsonRequestBehavior.AllowGet);
        }
        //Tetiana
        //
        public JsonResult GetCategoriesClickedPerUser(string username)
        {
            var listCatForUser = adminBL.GetCategoriesClickedPerUser(username);
            return Json(listCatForUser, JsonRequestBehavior.AllowGet);
        }

        //Tetiana metode for å sjekke session
        private bool LoggedInBruker()
        {
            var bruker = Session["LoggedIn"];
            if (bruker!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult AdminLoggInn()
        {
            return View();

        }
        [HttpPost]
        public ActionResult AdminLoggInn(SuperuserDTO superuserDTO)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var adminuser = adminBL.SuperuserInDb(superuserDTO);
            if (adminuser!=null)
            {
                Session["AdminLoggedIn"] = adminuser.Username;
            return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                Session["AdminLoggedIn"] = null;
                ModelState.AddModelError("","Feil brukernavn eller passord");
                return View();
            }
        }

        public JsonResult GetBrukerNavn()
        {
            var name = Session["AdminLoggedIn"];
            if (name != null)
            {
                var jsonSerializer = new JavaScriptSerializer();
                return Json(name, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        
        public string LogOutAdmin()
        {
            Session["AdminLoggedIn"] = null;
            return new JavaScriptSerializer().Serialize(true);
        }

        //Dashboard
        public ActionResult Dashboard()
        {
            if (Session["AdminLoggedIn"] == null)
            {
                return RedirectToAction("AdminLoggInn");
            }
            return View();
            
        }
        
        //Johan Sakshaug
        [HttpGet]
        public PartialViewResult CategoryEditor() {
            ViewBag.Categories = adminBL.GetCategories();
            return PartialView();
        }
        
        //Johan Sakshaug
        [HttpGet]
        public ViewResult SubmitCategory() {
            return View();
        }

        //Johan Sakshaug
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddCategory(CategorySubmitDTO c)
        {
            if (Session["AdminLoggedIn"] == null) {
                return RedirectToAction("AdminLoggInn");
            }

            if (ModelState.IsValid) {
                try {
                    if (!adminBL.AddCategory(c))
                    {
                        ViewBag.Melding = "Kategorinavn finnes fra før";
                        return View("SubmitCategory");
                    }
                    else
                    {
                        ViewBag.Melding = "Kategori " + c.CategoryName + " er lagt til.";
                        ViewBag.Done = true;
                        return View("SubmitCategory");
                    }
                }
                catch (DataException e) {
                    ViewBag.Feil = "Databasefeil ved innlegging av kategori.";
                }
                ViewBag.Done = true;
                return View("Close");
            }
            
            return View("SubmitCategory", c);
        }

        //Johan Sakshaug
        //[HttpPost]
        public ActionResult DeleteCategory(int categoryId) {
            if (Session["AdminLoggedIn"] == null) {
                return RedirectToAction("AdminLoggInn");
            }

            try {
                adminBL.DeleteCategory(categoryId);
            }
            catch(DbUpdateException e) {
              
            }
            catch (DataException e) {
                ViewBag.Feil = "Databasefeil ved sletting av kategori.";
            }
            ViewBag.Categories = adminBL.GetCategories();
            return PartialView("CategoryEditor");
        }

        //Johan Sakshaug
        [HttpGet]
        public ActionResult SubcategoryEditor(int id) {
            if (Session["AdminLoggedIn"] == null) {
                return RedirectToAction("AdminLoggInn");
            }

            try {
                ViewBag.Subcategories = adminBL.GetSubcategories(id);
                ViewBag.CategoryName = adminBL.GetCategoryName(id);
            }
            catch (DataException e) {
                ViewBag.Feil = "Databasefeil ved henting av underkategorier.";
            }
            ViewBag.CategoryId = id;
            if(TempData["Melding"] != null) ViewBag.Melding = TempData["Melding"];
            if(TempData["Feil"] != null) ViewBag.Feil = TempData["Feil"];
            return View();
        }
        
        //Johan Sakshaug
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddSubcategory(int categoryId, SubcategorySubmitDTO s)
        {
            if (Session["AdminLoggedIn"] == null) {
                return RedirectToAction("AdminLoggInn");
            }

            if(ModelState.IsValid) {
                if (s.Name == null) {
                    TempData["Feil"] = "Underkategorinavn kan ikke være tomt";
                }
                else if (s.Instruction == null) {
                    TempData["Feil"] = "Instruksjon kan ikke være tom";
                }
                else {
                    try
                    {
                        adminBL.AddSubcategory(categoryId, s);
                        TempData["Melding"] = "Underkategori " + s.Name + " lagt til.";
                    }
                    catch (DataException e) {
                        TempData["Feil"] = "Databasefeil ved innlegging av underkategori.";
                    }
                }
            }
            return RedirectToAction("SubcategoryEditor", new { id = categoryId });
        }

        //Johan Sakshaug
        //[HttpDelete]
        public ActionResult DeleteSubcategory(int subId, int categoryId)
        {
            if (Session["AdminLoggedIn"] == null) {
                return RedirectToAction("AdminLoggInn");
            }

            try {
                adminBL.DeleteSubcategory(subId);
                TempData["Melding"] = "Underkategori er slettet";
            }
            catch (DataException e) {
                System.Diagnostics.Trace.WriteLine(e.GetBaseException().Message);
                TempData["Feil"] = "Databasefeil ved sletting av underkategori.";
            }
            return RedirectToAction("SubcategoryEditor", new { id = categoryId});
        }

        //Johan Sakshaug
        //[HttpPost]
        public ActionResult UpdateInstruction(int subId, string instruction, int categoryId)
        {
            if (Session["AdminLoggedIn"] == null)
            {
                return RedirectToAction("AdminLoggInn");
            }

            try
            {
                adminBL.UpdateInstruction(subId, instruction);
                TempData["Melding"] = "Instruks er oppdatert";
            }
            catch (DataException e)
            {
                TempData["Feil"] = "Databasefeil ved oppdatering av instruks.";
            }
            return RedirectToAction("SubcategoryEditor", new { id = categoryId });
        }

        
        //Tetiana Bruker statistikk
        public ActionResult UserStatistics()
        {
            var listOfTopCat = adminBL.GetAllUsernamesCasesList();
            return View(listOfTopCat);
        }
        //Tetiana bruker detaljer
        public ActionResult UserDetails(string userName)
        {
            var listOverCases = adminBL.GetCaseDetails(userName);
            return View(listOverCases);
        }
        //Tetiana saks detaljer
        public ActionResult CaseDetails()
        {
            string username = null;
            var listOverCases = adminBL.GetCaseDetails(username);
            return View(listOverCases);
        }
    }
    
}