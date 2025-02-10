using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CWDocMgr.Data;
using CWDocMgr.Models;
using CWDocMgr.Services;

namespace CWDocMgr.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly IOCRService _ocrService;

        public DocumentController(ApplicationDbContext context, IConfiguration configuration, 
            IDocumentService documentService, ILogger<DocumentController> logger,
            IOCRService ocrService)
        {
            _context = context;
            _configuration = configuration;
            _documentService = documentService;
            _logger = logger;
            _ocrService = ocrService;
        }

        // GET: DocumentModels
        public async Task<IActionResult> Index()
        {
            System.Security.Claims.ClaimsIdentity user = HttpContext.User.Identities.ToArray()[0];

            IndexViewModel indexViewModel = new IndexViewModel
            {
                Documents = await _context.Documents
                    .Include(d => d.User)
                    .ToListAsync(),
                TotalPages = _documentService.GetTotalDocuments(),
                PageNumber = 0
            };

            _documentService.FillDocDateStrings(indexViewModel.Documents);

            return View(indexViewModel);
        }

        // GET: DocumentModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentModel = await _context.Documents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documentModel == null)
            {
                return NotFound();
            }

            string documentFilePath = _configuration["ClientDocumentStorePath"] + "/" + documentModel.DocumentName;
            documentModel.DocumentName = documentFilePath;

            return View(documentModel);
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
            _documentService.UploadDocuments(model, files, HttpContext.User);

            // redirect to home page
            return Redirect("/Document");
        }



        // GET: DocumentModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentModel = await _context.Documents.FindAsync(id);
            if (documentModel == null)
            {
                return NotFound();
            }
            return View(documentModel);
        }

        // POST: DocumentModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,DocumentName,OriginalDocumentName,DocumentDate")] DocumentModel documentModel)
        {
            if (id != documentModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documentModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentModelExists(documentModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(documentModel);
        }

        // GET: DocumentModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentModel = await _context.Documents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documentModel == null)
            {
                return NotFound();
            }

            return View(documentModel);
        }

        // POST: DocumentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _documentService.DeleteDocument(id);

            return RedirectToAction(nameof(Index));
        }

        // POST: DocumentModels/OCR/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<IActionResult> Ocr(int? id)
        {
            var documentModel = await _context.Documents.FindAsync(id);
            if (documentModel == null)
            {
                return NotFound();
            }

            DateTime startTime = DateTime.Now;

            //_documentService.OcrDocument(documentModel);
            _logger.LogInformation($"Thread {Thread.CurrentThread.ManagedThreadId}: OCRing file {documentModel.DocumentName}");


            // Extract file name from whatever was posted by browser
            //var originalFileName = System.IO.Path.GetFileName(documentModel.DocumentName);
            string imageFileExtension = Path.GetExtension(documentModel.DocumentName);

            //var fileName = Guid.NewGuid().ToString();

            // set up the image file (input) path
            //string imageFilePath = Path.Combine(_configuration["ImageFilePath"], documentModel.DocumentName);
            string imageFilePath = _documentService.GetDocFilePath(documentModel.DocumentName);
            //imageFilePath += imageFileExtension;

            //_debugLogger.Info($"ImageFilePath: {imageFilePath}");
            //_debugLogger.Info($"Current: {Directory.GetCurrentDirectory()}");

            // set up the text file (output) path
            //string textFilePath = Path.Combine(_configuration["TextFilePath"], documentModel.DocumentName);
            string textFilePath = imageFilePath.Split('.')[0];
            //textFilePath += ".txt";


            // If file with same name exists delete it
            if (System.IO.File.Exists(textFilePath))
            {
                System.IO.File.Delete(textFilePath);
            }

            //// Create new local file and copy contents of uploaded file
            //try
            //{
            //    using (var localFile = System.IO.File.OpenWrite(imageFilePath))
            //    using (var uploadedFile = files[0].OpenReadStream())
            //    {
            //        uploadedFile.CopyTo(localFile);
            //    }
            //}
            //catch (Exception)
            //{
            //    _debugLogger.Debug($"Couldn't write file {imageFilePath}");
            //    // HANDLE ERROR
            //}

            string errorMsg = "";

            if (imageFileExtension.ToLower() == ".pdf")
            {
                await _ocrService.OCRPDFFile(imageFilePath, textFilePath + ".tif", "eng");

            }
            else
            {
                errorMsg = await _ocrService.OCRImageFile(imageFilePath, textFilePath, "eng");
            }

            string textFileName = textFilePath + ".txt";
            string ocrText = "";
            try
            {
                ocrText = System.IO.File.ReadAllText(textFileName);
            }
            catch (Exception)
            {
                _logger.LogDebug($"Couldn't read text file {textFileName}");
            }

            if (ocrText == "")
            {
                if (errorMsg == "")
                    ocrText = "No text found.";
                else
                    ocrText = errorMsg;
            }

            // update model for display of ocr'ed data
            OcrFileModel ocrFileModel = new OcrFileModel
            {
                OriginalFileName = documentModel.OriginalDocumentName,
                CacheFilename = imageFilePath,
                Language = "eng",
                Languages = _ocrService.SetupLanguages()
            };

            
            TimeSpan ts = (DateTime.Now - startTime);
            string duration = ts.ToString(@"hh\:mm\:ss");

            //_ocrService.Cleanup(_configuration["ImageFilePath"], _configuration["TextFilePath"]);

            _logger.LogInformation($"Thread {Thread.CurrentThread.ManagedThreadId}: Finished processing file {documentModel.OriginalDocumentName} Elapsed time: {duration}");
            //_debugLogger.Debug($"Leaving HomeController.Index()");


            //return View(documentModel);
            return StatusCode(200);
        }

        [NonAction]
        private bool DocumentModelExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
