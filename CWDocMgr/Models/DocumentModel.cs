using CWDocMgr.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWDocMgr.Models
{
    public record DocumentModel
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required long DocumentDate { get; set; }
    }
}
