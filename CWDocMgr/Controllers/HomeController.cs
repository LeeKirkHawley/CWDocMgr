using DocMgrLib.Data;
using CWDocMgr.Models;
using DocMgrLib.Services;
using Microsoft.AspNetCore.Mvc;
using CWDocMgr.Services;
using DocMgrLib.Models;

namespace CWDocMgr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IDocumentService _documentService;
        IConfiguration _configuration;
        ApplicationDbContext _applicationDbContext;
        IAccountService _accountService;

        public HomeController(ILogger<HomeController> logger, 
            IDocumentService documentService, 
            IConfiguration configuration,
            ApplicationDbContext applicationDbContext, 
            IAccountService accountService)
        {
            _logger = logger;
            _documentService = documentService;
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
            _accountService = accountService;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var user = _accountService.LoggedInUser;

            if (user == null)
            {
                //return Redirect("/Identity/Account/Login");
                return Redirect("/Home/Login");
                //_accountService.Login("Kirk", "pwd");

            }

            //int totalDocuments = _documentService.GetTotalDocuments();
            //IEnumerable<DocumentModel> documents = _documentService.GetDocuments(page, pageSize);

            //IndexViewModel indexViewModel = new IndexViewModel
            //{
            //    Documents = documents,
            //    PageNumber = page,
            //    TotalPages = totalDocuments / pageSize + 1
            //};


            return View(new HomeViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            //LoginViewModel model = new LoginViewModel();

            return View();

            //var user = HttpContext.User.Identities.ToArray()[0];
            //if (!user.IsAuthenticated)
            //{
            //    return RedirectToAction("login", "account");
            //}

            //UploadDocsViewModel model = new UploadDocsViewModel();

            //return View(model);
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            UserModel user = _accountService.Login(username, password);

            if (user != null)
            {
                // Redirect to a different page on successful login
                return RedirectToAction("Index", "Home");
            }

            // Return the view with an error message on failed login
            ViewBag.ErrorMessage = "Invalid username or password";
            return View();
        }

        //[HttpPost]
        //public ActionResult UploadDoc(UploadDocsViewModel model, IFormFile[] files)
        //{
        //    _documentService.UploadDocuments(model, files, HttpContext.User);

        //    // redirect to home page
        //    return Redirect("/");
        //}



        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
