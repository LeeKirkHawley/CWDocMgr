using CWDocMgr.Controllers;
using CWDocMgr.Data;
using CWDocMgr.Models;
using Microsoft.EntityFrameworkCore;

namespace CWDocMgr.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        ApplicationDbContext _applicationDbContext;

        public DocumentService(ILogger<DocumentService> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<DocumentModel> GetDocuments(int page, int pageSize) 
        {
            // get paginated Documents      
            List<DocumentModel> docList = _applicationDbContext.Documents.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return docList;
        }

        public int GetTotalDocuments()
        {
            // get the total number of documents in the Documents table
            return _applicationDbContext.Documents.Count();
        }

        public DocumentModel CreateDocument(Microsoft.AspNetCore.Identity.IdentityUser user, string originalFileName, string documentFilePath)
        {

            DocumentModel newDoc = _applicationDbContext.Documents.Add(new DocumentModel
            {
                UserId = user.Id,
                DocumentName = Path.GetFileName(documentFilePath),
                OriginalDocumentName = originalFileName,
                DocumentDate = DateTime.Now.Ticks
            }).Entity;

            try
            {
                _applicationDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

            return newDoc;
        }



    }
}
