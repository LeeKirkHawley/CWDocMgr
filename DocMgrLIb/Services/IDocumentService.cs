using DocMgrLib.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DocMgrLib.Services
{
    public interface IDocumentService
    {
        IEnumerable<DocumentModel> GetDocuments(int page, int pageSize);
        int GetTotalDocuments();

        DocumentModel CreateDocument(UserModel user, string originalFileName, string documentFilePath);

        void DeleteDocument(int id);

        void UploadDocuments(UploadDocsViewModel model, IFormFile[] files, ClaimsPrincipal User);
        ObservableCollection<DocumentGridVM> UploadDocuments(string[] files, ClaimsPrincipal User);

        void FillDocDateStrings(IEnumerable<DocumentModel> docList);

        string GetDocFilePath(string fileName);

    }
}
