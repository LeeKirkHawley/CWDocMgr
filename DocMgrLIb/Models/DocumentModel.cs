using DocMgrLib.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocMgrLib.Models
{
    public record DocumentModel
    {

        public int Id { get; set; }
        public required int UserId { get; set; }
        //[ForeignKey("UserId")]
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required DateTime DocumentDate { get; set; }
    }
}
