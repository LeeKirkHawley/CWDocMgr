using CWDocMgr.Models;
using System.Security.Claims;

namespace CWDocMgr.Services
{
    public interface IDocumentService
    {
        IEnumerable<DocumentModel> GetDocuments(int page, int pageSize);
        int GetTotalDocuments();

        DocumentModel CreateDocument(Microsoft.AspNetCore.Identity.IdentityUser user, string originalFileName, string documentFilePath);

        void DeleteDocument(int id);

        void UploadDocuments(UploadDocsViewModel model, IFormFile[] files, ClaimsPrincipal User);

        void FillDocDateStrings(IEnumerable<DocumentModel> docList);
    }
}
