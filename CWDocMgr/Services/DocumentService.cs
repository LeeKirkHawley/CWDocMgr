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

        public IEnumerable<DocumentModel> GetDocuments() 
        {
            List<DocumentModel> docList = _applicationDbContext.Documents.ToList();
            return docList;
        }
    }
}
