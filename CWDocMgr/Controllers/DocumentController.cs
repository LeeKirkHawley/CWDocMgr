using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CWDocMgr.Data;
using CWDocMgr.Models;

namespace CWDocMgr.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _settings;

        public DocumentController(ApplicationDbContext context, IConfiguration settings)
        {
            _context = context;
            _settings = settings;
        }

        // GET: DocumentModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Documents.ToListAsync());
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

            string documentFilePath = Path.Combine(_settings["UploadFilePath"], documentModel.documentName);
            documentModel.documentName = documentFilePath;

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
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentModelExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
}
