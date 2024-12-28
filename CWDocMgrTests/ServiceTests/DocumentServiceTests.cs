using CWDocMgr.Data;
using CWDocMgr.Models;
using CWDocMgr.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CWDocMgrTests.ServiceTests
{
    public class DocumentServiceTests
    {
        private readonly Mock<ILogger<DocumentService>> _loggerMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly DocumentService _documentService;

        public DocumentServiceTests()
        {
            _loggerMock = new Mock<ILogger<DocumentService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(null, options);
            _documentService = new DocumentService(_loggerMock.Object, _dbContext);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_dbContext.Documents.Any())
            {
                var documents = new List<DocumentModel>
                {
                    new DocumentModel { Id = 1, UserId = 1, DocumentName = "Doc1", OriginalDocumentName = "OriginalDoc1", DocumentDate = 20230101 },
                    new DocumentModel { Id = 2, UserId = 2, DocumentName = "Doc2", OriginalDocumentName = "OriginalDoc2", DocumentDate = 20230102 },
                    new DocumentModel { Id = 3, UserId = 3, DocumentName = "Doc3", OriginalDocumentName = "OriginalDoc3", DocumentDate = 20230103 }
                };

                _dbContext.Documents.AddRange(documents);
                _dbContext.SaveChanges();
            }
        }

        [Fact]
        public void GetDocuments_ReturnsCorrectDocuments()
        {
            // Arrange
            int page = 1;
            int pageSize = 2;

            // Act
            var result = _documentService.GetDocuments(page, pageSize);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Doc1", result.First().DocumentName);
            Assert.Equal("Doc2", result.Last().DocumentName);
        }

        [Fact]
        public void GetTotalDocuments_ReturnsCorrectCount()
        {
            // Act
            var result = _documentService.GetTotalDocuments();

            // Assert
            Assert.Equal(3, result);
        }
    }
}

