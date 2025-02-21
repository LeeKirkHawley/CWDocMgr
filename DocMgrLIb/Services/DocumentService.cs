using DocMgrLib.Data;
using DocMgrLib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace DocMgrLib.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private readonly IConfiguration _configuration;
        ApplicationDbContext _applicationDbContext;

        public DocumentService(ILogger<DocumentService> logger, ApplicationDbContext applicationDbContext,
            IConfiguration configuration)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
        }

        public IEnumerable<DocumentModel> GetDocuments(UserModel user, int page, int pageSize)
        {
            List<DocumentModel> docList = _applicationDbContext.Documents
                .Where(user => user.Id == user.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            FillDocDateStrings(docList);

            return docList;
        }

        public void FillDocDateStrings(IEnumerable<DocumentModel> docList)
        {
            foreach (DocumentModel doc in docList)
            {
                var documentDateTime = DateTime.FromBinary(doc.DocumentDate);
                doc.Date = documentDateTime.ToString("MM/dd/yyyy");
            }
        }

        public int GetTotalDocuments()
        {
            // get the total number of documents in the Documents table
            return _applicationDbContext.Documents.Count();
        }

        public DocumentModel CreateDocument(UserModel user, string originalFileName, string documentFilePath)
        {

            DocumentModel newDoc = _applicationDbContext.Documents.Add(new DocumentModel
            {
                UserId = user.Id,
                DocumentName = Path.GetFileName(documentFilePath),
                OriginalDocumentName = originalFileName,
                DocumentDate = DateTime.Now.Ticks
            }).Entity;

            try
            {
                _applicationDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

            return newDoc;
        }

        public async void DeleteDocument(int id)
        {
            var documentModel = await _applicationDbContext.Documents.FindAsync(id);
            if (documentModel != null)
            {
                _applicationDbContext.Documents.Remove(documentModel);
            }

            await _applicationDbContext.SaveChangesAsync();

            string documentFilePath = Path.Combine(_configuration["DocumentStorePath"], documentModel.DocumentName);
            if (File.Exists(documentFilePath))
            {
                _logger.LogInformation($"Deleting file {documentFilePath}");
                File.Delete(documentFilePath);
            }
            else
            {
                _logger.LogDebug($"Failed to delete file {documentFilePath}");
            }
        }

        public void UploadDocuments(UploadDocsViewModel model, IFormFile[] files, UserModel User)
        {
            //ClaimsIdentity identity = User.Identities.ToArray()[0];
            //if (!identity.IsAuthenticated)
            //{
            //    throw new Exception("user is not logged in.");
            //}

            DateTime startTime = DateTime.Now;

            foreach (var file in files)
            {
                // Extract file name from whatever was posted by browser
                var originalFileName = Path.GetFileName(file.FileName);
                string imageFileExtension = Path.GetExtension(originalFileName);

                var fileName = Guid.NewGuid().ToString() + imageFileExtension;

                // set up the document file (input) path
                string documentFilePath = GetDocFilePath(fileName);

                // If file with same name exists
                if (File.Exists(documentFilePath))
                {
                    throw new Exception($"Document {documentFilePath} already exists!");
                }

                // Create new local file and copy contents of uploaded file
                try
                {
                    using (var localFile = File.OpenWrite(documentFilePath))
                    using (var uploadedFile = file.OpenReadStream())
                    {
                        uploadedFile.CopyTo(localFile);
                        bool fileExists = File.Exists(documentFilePath);

                        // update model for display of ocr'ed data
                        model.OriginalFileName = originalFileName;

                        DateTime finishTime = DateTime.Now;
                        TimeSpan ts = (finishTime - startTime);
                        string duration = ts.ToString(@"hh\:mm\:ss");

                        _logger.LogInformation($"Thread {Thread.CurrentThread.ManagedThreadId}: Finished uploading document {file} to {localFile} Elapsed time: {duration}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug($"Couldn't write file {documentFilePath}");
                    // HANDLE ERROR
                    throw;
                }

                UserModel user = _applicationDbContext.Users.Where(u => u.Id == User.Id).FirstOrDefault();
                CreateDocument(user, originalFileName, documentFilePath);
            }
        }

        public ObservableCollection<DocumentGridVM> UploadDocuments(string[] files, UserModel User)
        {
            //ClaimsIdentity identity = User.Identities.ToArray()[0];
            //if (!identity.IsAuthenticated)
            //{
            //    throw new Exception("user is not logged in.");
            //}

            DateTime startTime = DateTime.Now;

            ObservableCollection<DocumentGridVM> collection = new ObservableCollection<DocumentGridVM>();

            foreach (var file in files)
            {

                // Extract file name from whatever was posted by browser
                var originalFileName = file;
                string imageFileExtension = Path.GetExtension(originalFileName);

                var fileName = Guid.NewGuid().ToString() + imageFileExtension;

                // set up the document file (input) path
                string documentFilePath = GetDocFilePath(fileName);

                // If file with same name exists
                if (File.Exists(documentFilePath))
                {
                    throw new Exception($"Document {documentFilePath} already exists!");
                }

                // Create new local file and copy contents of uploaded file
                try
                {
                    using (var localFile = File.OpenWrite(documentFilePath))
                    using (FileStream uploadedFile = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        uploadedFile.CopyTo(localFile);
                        bool fileExists = File.Exists(documentFilePath);

                        DateTime finishTime = DateTime.Now;
                        TimeSpan ts = (finishTime - startTime);
                        string duration = ts.ToString(@"hh\:mm\:ss");

                        //_logger.LogInformation($"Thread {Thread.CurrentThread.ManagedThreadId}: Finished uploading document {file} to {localFile} Elapsed time: {duration}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug($"Couldn't write file {documentFilePath}");
                    // HANDLE ERROR
                    throw;
                }



                UserModel user = _applicationDbContext.Users.Where(u => u.Id == User.Id).FirstOrDefault();
                DocumentModel newDocument = CreateDocument(user, originalFileName, documentFilePath);

                DocumentGridVM doc = new DocumentGridVM { 
                    Id = newDocument.Id,
                    UserName = user.UserName,
                    OriginalDocumentName = originalFileName,
                    DocumentName = fileName,
                    DocumentDate = DateTime.Now.Ticks
                };

                collection.Add(doc);
            }

            return collection;
        }

        public string GetDocFilePath(string fileName)
        {
            return Path.Combine(_configuration["DocumentStorePath"], fileName);
        }

    }
}
