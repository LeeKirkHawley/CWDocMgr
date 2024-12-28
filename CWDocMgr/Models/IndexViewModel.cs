namespace CWDocMgr.Models
{
    public class IndexViewModel
    {
        public IndexViewModel() { }

        public IEnumerable<DocumentModel>? Documents { get; set; }

        public int TotalPages;
        public int PageNumber;

        //public List<DocumentModel> Documents = new List<DocumentModel>{
        //    new DocumentModel {
        //        Id = 1,
        //        UserId = 1,
        //        DocumentName = "Doc1",
        //        OriginalDocumentName = "OriginalDoc1",
        //        DocumentDate = DateTime.Now.ToFileTime(),
        //    },
        //    new DocumentModel {
        //        Id = 2,
        //        UserId = 1,
        //        DocumentName = "Doc2",
        //        OriginalDocumentName = "OriginalDoc2",
        //        DocumentDate = DateTime.Now.ToFileTime(),
        //    }
        //};


    }
}
