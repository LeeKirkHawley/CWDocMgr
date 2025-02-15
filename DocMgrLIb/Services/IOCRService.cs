using DocMgrLib.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocMgrLib.Services
{
    public interface IOCRService
    {
        Task DoOcr(DocumentModel? documentModel);
        string OCRImageFile(string imageName, string outputBase, string language);
        Task<string> OCRPDFFile(string pdfName, string outputFile, string language);
        List<SelectListItem> SetupLanguages();
        void ImmediateCleanup(string imageFilePath, string imageFileExtension, string textFilePath);
        void Cleanup(string imageFilePath, string textFilePath);

    }
}
