using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocMgrLib.Models
{
    public record DocumentModelVM
    {
        public DocumentModelVM()
        {
            OCRText = "";
            User = null;
            DateString = null;
        }

        public int Id { get; set; }
        public required int UserId { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required DateTime DocumentDate { get; set; }

        public virtual UserModel? User { get; set; }
        public string? DateString { get; set; }
        public string OCRText { get; set; }
    }
}
