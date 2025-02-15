using DocMgrLib.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocMgrLib.Models
{
    public record DocumentModel
    {
        public DocumentModel()
        {
            OCRText = "";
            User = null;
            Date = null;
        }

        public int Id { get; set; }
        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserModel? User { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required long DocumentDate { get; set; }

        [NotMapped]
        public string? Date { get; set; }

        [NotMapped]
        public string OCRText { get; set; }

    }
}
