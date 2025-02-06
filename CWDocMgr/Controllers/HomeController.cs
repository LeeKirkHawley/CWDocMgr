using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using CWDocMgr.Data;
using CWDocMgr.Models;
using CWDocMgr.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            int totalDocuments = _documentService.GetTotalDocuments();
            IEnumerable<DocumentModel> documents = _documentService.GetDocuments(page, pageSize);

            IndexViewModel indexViewModel = new IndexViewModel
            {
                Documents = documents,
                PageNumber = page,
                TotalPages = totalDocuments / pageSize + 1
            };


            return View(indexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UploadDoc()
        {

            var user = HttpContext.User.Identities.ToArray()[0];
            if (!user.IsAuthenticated)
            {
                return RedirectToAction("login", "account");
            }

            UploadDocsViewModel model = new UploadDocsViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult UploadDoc(UploadDocsViewModel model, IFormFile[] files)
        {
            UploadDocuments(model, files, HttpContext.User);

            // redirect to home page
            return Redirect("/");
        }

        public void UploadDocuments(UploadDocsViewModel model, IFormFile[] files, ClaimsPrincipal User)
        {
            ClaimsIdentity identity = User.Identities.ToArray()[0];
            if (!identity.IsAuthenticated)
            {
                throw new Exception("user is not logged in.");
            }


            DateTime startTime = DateTime.Now;

            string file = "";
            try
            {
                file = $"{files[0].FileName}";
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception reading file name.");
            }

            _logger.LogInformation($"Thread {Thread.CurrentThread.ManagedThreadId}: Processing file {file}");


            // Extract file name from whatever was posted by browser
            var originalFileName = System.IO.Path.GetFileName(files[0].FileName);
            string imageFileExtension = Path.GetExtension(originalFileName);

            var fileName = Guid.NewGuid().ToString() + imageFileExtension;

            // set up the document file (input) path
            //var webRootPath = _configuration["WebRootPath"];
            string documentFilePath = Path.Combine(_configuration["ServerDocumentStorePath"], fileName);
            //documentFilePath += imageFileExtension;

            // If file with same name exists
            if (System.IO.File.Exists(documentFilePath))
            {
                throw new Exception($"Document {documentFilePath} already exists!");
            }

            // Create new local file and copy contents of uploaded file
            try
            {
                using (var localFile = System.IO.File.OpenWrite(documentFilePath))
                using (var uploadedFile = files[0].OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                    bool fileExists = System.IO.File.Exists(documentFilePath);

                    // update model for display of ocr'ed data
                    model.OriginalFileName = originalFileName;

                    DateTime finishTime = DateTime.Now;
                    TimeSpan ts = (finishTime - startTime);
                    string duration = ts.ToString(@"hh\:mm\:ss");

                    _logger.LogInformation($"Thread {Thread.CurrentThread.ManagedThreadId}: Finished uploading document {file} to {localFile} Elapsed time: {duration}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Couldn't write file {documentFilePath}");
                // HANDLE ERROR
                throw;
            }

            var doesFileExist = System.IO.File.Exists(documentFilePath);
            //string errorMsg = "";

            //if (imageFileExtension.ToLower() == ".pdf") {
            //    await _ocrService.OCRPDFFile(imageFilePath, textFilePath + ".tif", "eng");

            //}
            //else {
            //    errorMsg = await _ocrService.OCRImageFile(imageFilePath, textFilePath, "eng");
            //}

            //string textFileName = textFilePath + ".txt";
            //string ocrText = "";
            //try {
            //    ocrText = System.IO.File.ReadAllText(textFileName);
            //}
            //catch (Exception ex) {
            //    _debugLogger.Debug($"Couldn't read text file {textFileName}");
            //}

            //if (ocrText == "") {
            //    if (errorMsg == "")
            //        ocrText = "No text found.";
            //    else
            //        ocrText = errorMsg;
            //}

            Microsoft.AspNetCore.Identity.IdentityUser user = _applicationDbContext.Users.First();
            _documentService.CreateDocument(user, originalFileName, documentFilePath);
        }


        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
