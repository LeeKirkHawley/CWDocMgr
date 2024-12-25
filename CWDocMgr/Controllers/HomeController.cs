using System.Diagnostics;
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

        public IActionResult Index()
        {
            IEnumerable<DocumentModel> documents = _documentService.GetDocuments();

            IndexViewModel indexViewModel = new IndexViewModel
            {
                Documents = documents
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
