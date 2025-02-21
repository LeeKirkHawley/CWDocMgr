using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocMgrLib.Data;
using CWDocMgr.Models;
using DocMgrLib.Services;
using DocMgrLib.Models;
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
        private readonly IFileService _fileService;
        private readonly IAccountService _accountService;

        public DocumentController(ApplicationDbContext context, IConfiguration configuration, 
            IDocumentService documentService, ILogger<DocumentController> logger,
            IOCRService ocrService, IFileService fileService, IAccountService accountService)
        {
            _context = context;
            _configuration = configuration;
            _documentService = documentService;
            _logger = logger;
            _ocrService = ocrService;
            _fileService = fileService;
            _accountService = accountService;
        }

        // GET: DocumentModels
        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel
            {
                Documents = await _context.Documents
                    .Include(d => d.User)
                    .ToListAsync(),
                TotalPages = _documentService.GetTotalDocuments(),
                PageNumber = 0,
                User = _accountService.LoggedInUser
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

            DocumentModel? documentModel = await _context.Documents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documentModel == null)
            {
                return NotFound();
            }

            string documentFilePath = _configuration["ClientDocumentStorePath"] + "/" + documentModel.DocumentName;
            documentModel.DocumentName = documentFilePath;

            string ocrFilePath = _fileService.GetOcrFilePath(documentFilePath);
            if(System.IO.File.Exists(ocrFilePath))
            {
                try
                {
                    documentModel.OCRText = System.IO.File.ReadAllText(ocrFilePath);
                }
                catch (Exception)
                {
                    _logger.LogDebug($"Couldn't read text file {ocrFilePath}");
                }
            }

            return View(documentModel);
        }

        [HttpGet]
        public IActionResult UploadDoc()
        {
            UserModel user = _accountService.LoggedInUser;
            if (user == null)
            {
                return RedirectToAction("login", "account");
            }

            var model = new UploadDocsViewModel {
                User = user,
                OriginalFileName = "",
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UploadDoc(UploadDocsViewModel model, IFormFile[] files)
        {
            var user = _context.Users.Find(model.User.Id);
            _documentService.UploadDocuments(model, files, user);

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
            DocumentModel? documentModel = await _context.Documents.FindAsync(id);
            if (documentModel == null)
            {
                return NotFound();
            }

            await _ocrService.DoOcr(documentModel);


            return StatusCode(200);
        }


        [NonAction]
        private bool DocumentModelExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
