namespace DocMgrLib.Services
{
    public interface IFileService
    {
        string GetDocFilePath(string fileName);
        string GetOcrFilePath(string fileName);
        string GetWorkFilePath();

    }
}
