using Microsoft.AspNetCore.Mvc.Rendering;

namespace CWDocMgr.Services
{
    public interface IOCRService
    {
        Task<string> OCRImageFile(string imageName, string outputBase, string language);
        Task<string> OCRPDFFile(string pdfName, string outputFile, string language);
        List<SelectListItem> SetupLanguages();
        void ImmediateCleanup(string imageFilePath, string imageFileExtension, string textFilePath);
        void Cleanup(string imageFilePath, string textFilePath);
    }
}
