namespace CWDocMgr.Models
{
    public record DocumentModel
    {
        public required int Id { get; set; }
        public required int UserId { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required long DocumentDate { get; set; }
        public required string documentName { get; set; }
    }
}
