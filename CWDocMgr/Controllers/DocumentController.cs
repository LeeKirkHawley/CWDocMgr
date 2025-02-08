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

        public DocumentController(ApplicationDbContext context, IConfiguration configuration, 
            IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _context = context;
            _configuration = configuration;
            _documentService = documentService;
            _logger = logger;
        }

        // GET: DocumentModels
        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel
            {
                Documents = await _context.Documents.ToListAsync(),
                TotalPages = _documentService.GetTotalDocuments(),
                PageNumber = 0
            };
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

        // GET: DocumentModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocumentModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,DocumentName,OriginalDocumentName,DocumentDate")] DocumentModel documentModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(documentModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(documentModel);
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
            var documentModel = await _context.Documents.FindAsync(id);
            if (documentModel != null)
            {
                _context.Documents.Remove(documentModel);
            }

            await _context.SaveChangesAsync();

            string documentFilePath = Path.Combine(_configuration["ServerDocumentStorePath"], documentModel.DocumentName);
            if (System.IO.File.Exists(documentFilePath))
            {
                _logger.LogInformation($"Deleting file {documentFilePath}");
                System.IO.File.Delete(documentFilePath);
            }
            else
            {
                _logger.LogDebug($"Failed to delete file {documentFilePath}");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DocumentModelExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
