namespace CWDocMgr.Models
{
    public record DocumentModel
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required long DocumentDate { get; set; }
    }
}
