using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocMgrLib.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetOcrFilePath(string fileName)
        {
            string ocrFilePath = Path.GetFileNameWithoutExtension(fileName);
            ocrFilePath = Path.Combine(_configuration["OcrTextPath"], ocrFilePath);
            ocrFilePath += ".txt";
            return ocrFilePath;
        }

        public string GetWorkFilePath()
        {
            return _configuration["WorkFolderPath"];
        }

        public string GetDocFilePath(string fileName)
        {
            return Path.Combine(_configuration["ServerDocumentStorePath"], fileName);
        }
    }
}
