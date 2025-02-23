using DocMgrLib.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using System.Security.Claims;

namespace DocMgrLib.Services
{
    public interface IDocumentService
    {
        IEnumerable<DocumentModel> GetDocuments(UserModel user, int page, int pageSize);
        int GetTotalDocuments();
        DocumentModel CreateDocument(UserModel user, string originalFileName, string documentFilePath);
        void DeleteDocument(int id);
        void UploadDocuments(UploadDocsViewModel model, IFormFile[] files, UserModel User);
        ObservableCollection<DocumentGridVM> UploadDocuments(string[] files, UserModel User);
        //void FillDocDateStrings(IEnumerable<DocumentModel> docList);
        string GetDocFilePath(string fileName);
        IEnumerable<DocumentModelVM> BuildDocModelVMList(IEnumerable<DocumentModel> docModels);
    }
}
