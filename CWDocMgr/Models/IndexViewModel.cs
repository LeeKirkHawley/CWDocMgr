using DocMgrLib.Models;

namespace CWDocMgr.Models
{
    public record IndexViewModel
    {
        public IndexViewModel() { }



        public IEnumerable<DocumentModel>? Documents { get; set; }

        public int TotalPages;
        public int PageNumber;
    }
}
