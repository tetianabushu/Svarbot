using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Svarbot.Controllers
{
    public class HomeController : Controller
    {
        private SvarbotBL svarbotBL;
        public HomeController()
        {
            svarbotBL = new SvarbotBL();
        }
        public HomeController(SvarbotBL svarbotBusL)
        {
            svarbotBL = svarbotBusL;
        }

        // GET: Home
        public ActionResult Index() {
           return View();
        }
        
        public ActionResult Hjelp() {
            return View();
        }

        public ActionResult Bestille()
        {
            return View();
        }

        public ActionResult Endre()
        {
            return View();
        }

        public ActionResult GodStart()
        {
            return View();
        }

        public ActionResult KontaktOss()
        {
            return View();
        }

        public ActionResult NyttigInformasjon()
        {
            return View();
        }

        //Margrete Sander
        //Johan Sakshaug
        //creates view for submitting a case
        [HttpGet]
        public ActionResult Skjema(string categoryId)
        {
            SkjemaDTO skjema = new SkjemaDTO();
            //hvis id er null, åpne generelt skjema
            // hvis id er ikke null, hente navn på kategori
            if (categoryId != null)
            {
                var numValOfCatId = -1;
                numValOfCatId = Convert.ToInt32(categoryId);
                KategoriDTO categoryToShowinSkjema = new KategoriDTO();
                categoryToShowinSkjema = svarbotBL.GetCategoryById(numValOfCatId);
                //skjema.CategoryId = categoryToShowinSkjema.id;
                skjema.CategoryName = categoryToShowinSkjema.name;
            }
            //genererer random id for skjema?? hvorfor trenger vi det? det lagres ingen sted
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] rndBytes = new byte[4];
            rng.GetBytes(rndBytes);
            int randomId = BitConverter.ToInt16(rndBytes, 0);
            skjema.Id = randomId;
            if (skjema.Id < 0) {
                skjema.Id = skjema.Id * (-1);
            }

            //check if user is logged in
            if (Session["LoggedInUsername"] != null) {
                //set username in the form
                skjema.Username = (string)Session["LoggedInUsername"];
            }

            return View(skjema);
        }
        
        //Johan Sakshaug
        //submits a case
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Skjema(SkjemaDTO inCase)
        {
            //set the case's date of submission to today
            inCase.DateSubmitted = DateTime.Now;
            if (ModelState.IsValid) {
                try {
                    svarbotBL.SubmitCase(inCase);
                    ViewBag.Innmeldt = "Sak meldt inn";
                    return View();
                }
                catch(DataException e) {
                    ViewBag.Melding = "Databasefeil";
                    System.Diagnostics.Trace.WriteLine(e.GetBaseException().Message);
                }
            }
            else {
                ViewBag.Melding = "Validering feilet hos serveren";
            }
            return View(inCase);
        }
        
        //Johan Sakshaug
        [HttpGet]
        public ActionResult LongInstruction(int? id) {
            LongInstructionDTO longInstruction = new LongInstructionDTO();

            //check if the url has id parameter set
            if (id.HasValue) {
                //get long instructions from db given id
                longInstruction.id = id.Value;
                longInstruction.text = svarbotBL.GetLongInstructionText(id.Value);
                longInstruction.title = svarbotBL.GetLongInstructionTitle(id.Value);
                return View(longInstruction);
            }
            else {
                longInstruction.text = "";
                longInstruction.title = "";
                return View(longInstruction);
            }
        }

        #region Navigasjon
        /*Navigasjons funksjonalitet*/
        //Tetiana
        //returnerer to typer av kategorier pc og service
        public PartialViewResult PartialChat()
        {
            List <CategoryTypeDTO> kategoriListeFraDb = new List<CategoryTypeDTO>();
            kategoriListeFraDb = svarbotBL.GetTypes();
            return PartialView(kategoriListeFraDb);
        }

        // Tetiana
        public bool UserLoggedIn()
        {
            if(Session["LoggedIn"] != null)
            {
                var loggedIn = (bool)Session["LoggedIn"];
                return loggedIn;
            }
            return false;
        }

        //Tetiana
        //returnerer liste av hoved kategorier basert på type
        public JsonResult GetMainCategories(int typeId, int? count, string searchText)
        {
            List<KategoriDTO> kategoriListeFraDb = svarbotBL.GetMainCategories(typeId, count, searchText);
            return Json(kategoriListeFraDb, JsonRequestBehavior.AllowGet);
        }

        //Tetiana
        //henter liste over alle subcategories for en hoved category
        public JsonResult GetUndercategories(int categoryId, string searchText)
        {
            var username = (string)Session["LoggedInUsername"];
            MainCategoryDetailsDTO underKategoriList = svarbotBL.GetAllUndercatFromDb(categoryId, searchText, username);
            return Json(underKategoriList, JsonRequestBehavior.AllowGet);
        }

        //Tetiana
        //henter liste over alle subcategories for en hoved category
        public JsonResult OpenInstruksVeiledning(int underCategoryId)
        {
            var username = (string)Session["LoggedInUsername"];
            var underKategori = svarbotBL.GetUndercategoryDetails(underCategoryId, username);
            return Json(underKategori, JsonRequestBehavior.AllowGet);
        }
        /*Navigasjons funksjonalitet*/

        #endregion

        //metode for å hente korte instruks, veiledning fra database
        public string GetInstruksVeiledning(int id)
        {
            InstruksVeiledningDTO shortInstructionsToReturn = svarbotBL.GetInstruksVeiledning(id);
            var jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(shortInstructionsToReturn);
            
        }

        //Johan Sakshaug
        //returns 1 as a json object if login is successful, 0 if not
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(string inUsername, string inPassword) {
            if (svarbotBL.userInDb(new UserDTO() { username = inUsername, password = inPassword})) {
                Session["LoggedIn"] = true;
                Session["LoggedInUsername"] = inUsername;
                return Json(new { status = 1, user = inUsername }, JsonRequestBehavior.AllowGet);
            }
            else {
                Session["LoggedIn"] = false;
                Session["LoggedInUsername"] = null;
                return Json(new { status = 0}, JsonRequestBehavior.AllowGet);
            }
        }

        //Johan Sakshaug
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrer(UserDTO inUser) {
            if (!ModelState.IsValid) {
                return View(inUser);
            }
            try {
                svarbotBL.registerUser(inUser);
            }
            catch (DataException e) {
                //handle exception
            }
            return View();
        }
        
        //Johan Sakshaug
        [HttpPost]
        public JsonResult Logout() {
            Session["LoggedIn"] = false;
            Session["LoggedInUsername"] = null;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //Admin view for testing før påloggings metode implementeres
        public ActionResult DashboardAdmin()
        {
            return View("../Admin/Dashboard.cshtml");
        }

        //Johan Sakshaug
        [HttpGet]
        public PartialViewResult LoggInnPartial() {
            return PartialView();
        }
        
        //Johan Sakshaug
        public PartialViewResult LoggedInAs() {
            return PartialView();
        }

        [HttpGet]
        //Johan Sakshaug
        public PartialViewResult LoggedInMenu() {
            return PartialView();
        }
        
        //Johan Sakshaug
        [HttpGet]
        public PartialViewResult Favorites() {
            return PartialView();
        }
        
        //Johan Sakshaug
        [HttpGet]
        public PartialViewResult SubmittedCases() {
            return PartialView();
        }
        
        //Johan Sakshaug
        [HttpPost]
        public void ToggleFavorite(int subcategoryId) {
            if (Session["LoggedInUsername"] != null) {
                svarbotBL.ToggleFavorite((string)Session["LoggedInUsername"], subcategoryId);
            }
        }

        //Johan Sakshaug
        //get a users favorite subcategories
        [HttpGet]
        public JsonResult GetFavoriteSubcategories() {
            string username = (string)Session["LoggedInUsername"];
            List<SubcategoryDTO> favSubcategories = svarbotBL.GetFavoriteSubcategories(username);
            return Json(favSubcategories, JsonRequestBehavior.AllowGet);
        }
        //Tetiana
        //henter navn av hoved kat by id
        public JsonResult GetMainCatName(int mainId)
        {
            var maincatname = svarbotBL.GetCategoryById(mainId);
            return Json(maincatname.name, JsonRequestBehavior.AllowGet);
        }

        //Johan Sakshaug
        //gets the cases submitted by the user currently logged in
        [HttpGet]
        public JsonResult GetSubmittedCases() {
            string username = (string)Session["LoggedInUsername"];
            List<SkjemaDTO> submittedCases = svarbotBL.GetSubmittedCases(username);
            return Json(submittedCases, JsonRequestBehavior.AllowGet);
        }        
    }
       
}