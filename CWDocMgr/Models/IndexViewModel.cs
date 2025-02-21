using DocMgrLib.Models;

namespace CWDocMgr.Models
{
    public record IndexViewModel
    {
        public UserModel User { get; set; }

        public IEnumerable<DocumentModel>? Documents { get; set; }

        public int TotalPages;
        public int PageNumber;
    }
}
