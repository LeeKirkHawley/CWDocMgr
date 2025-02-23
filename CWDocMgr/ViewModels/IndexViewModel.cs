using DocMgrLib.Models;

namespace CWDocMgr.ViewModels
{
    public record IndexViewModel
    {
        public UserModel? User { get; set; }

        public IEnumerable<DocumentModelVM>? Documents { get; set; }

        public int TotalPages;
        public int PageNumber;
    }
}
