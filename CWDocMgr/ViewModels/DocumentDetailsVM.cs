using DocMgrLib.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWDocMgr.ViewModels
{
    public record DocumentDetailsVM
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public virtual UserModel? User { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required string DocumentDate { get; set; }
        public string OCRText { get; set; }

    }
}
