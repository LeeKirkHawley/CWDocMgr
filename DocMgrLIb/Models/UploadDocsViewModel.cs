namespace DocMgrLib.Models
{
    public record UploadDocsViewModel
    {
        //public UploadDocsViewModel(UserModel user, string originalFileName = "")
        //{
        //    User = user;
        //    OriginalFileName = originalFileName;
        //}

        public UserModel User { get; set; }
        public string OriginalFileName { get; set; }
    }
}
