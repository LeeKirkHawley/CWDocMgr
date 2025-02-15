using DocMgrLib.Data;
using CWDocMgr.Models;
using DocMgrLib.Services;
using Microsoft.AspNetCore.Mvc;

namespace CWDocMgr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IDocumentService _documentService;
        IConfiguration _configuration;
        ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger, 
            IDocumentService documentService, 
            IConfiguration configuration,
            ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _documentService = documentService;
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var user = HttpContext.User.Identities.ToArray()[0];
            if (!user.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
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

        //[HttpGet]
        //public IActionResult UploadDoc()
        //{

        //    var user = HttpContext.User.Identities.ToArray()[0];
        //    if (!user.IsAuthenticated)
        //    {
        //        return RedirectToAction("login", "account");
        //    }

        //    UploadDocsViewModel model = new UploadDocsViewModel();

        //    return View(model);
        //}

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
