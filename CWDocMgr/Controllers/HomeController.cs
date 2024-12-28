using System.Diagnostics;
using System.Security.Principal;
using CWDocMgr.Data;
using CWDocMgr.Models;
using CWDocMgr.Services;
using Microsoft.AspNetCore.Mvc;

namespace CWDocMgr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IDocumentService _documentService;


        public HomeController(ILogger<HomeController> logger, IDocumentService documentService)
        {
            _logger = logger;
            _documentService = documentService;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var user = HttpContext.User.Identities.ToArray()[0];
            if (!user.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            int totalDocuments = _documentService.GetTotalDocuments();
            IEnumerable<DocumentModel> documents = _documentService.GetDocuments(page, pageSize);

            IndexViewModel indexViewModel = new IndexViewModel
            {
                Documents = documents,
                PageNumber = page,
                TotalPages = totalDocuments / pageSize
            };


            return View(indexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
