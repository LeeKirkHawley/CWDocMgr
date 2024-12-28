using CWDocMgr.Models;

namespace CWDocMgr.Services
{
    public interface IDocumentService
    {
        IEnumerable<DocumentModel> GetDocuments(int page, int pageSize);
        int GetTotalDocuments();
    }
}
