using DocMgrLib.Data;
using DocMgrLib.Models;
using DocMgrLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly Mock<IConfiguration> _configuration;

        public DocumentServiceTests()
        {
            _loggerMock = new Mock<ILogger<DocumentService>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(null, options);
            _configuration = new Mock<IConfiguration>();

            _documentService = new DocumentService(_loggerMock.Object, _dbContext, _configuration.Object);


            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_dbContext.Documents.Any())
            {
                var documents = new List<DocumentModel>
                {
                    new DocumentModel { Id = 1, UserId = "xxxx0f9c-183e-41ab-bca4-5104d887aaaa", DocumentName = "bdce0f9c-183e-41ab-bca4-5104d887aaaa.jpg", OriginalDocumentName = "OriginalDoc1", DocumentDate = 20230101 },
                    new DocumentModel { Id = 2, UserId = "xxxx0f9c-183e-41ab-bca4-5104d887bbbb", DocumentName = "bdce0f9c-183e-41ab-bca4-5104d887bbbb.jpg", OriginalDocumentName = "OriginalDoc2", DocumentDate = 20230102 },
                    new DocumentModel { Id = 3, UserId = "xxxx0f9c-183e-41ab-bca4-5104d887cccc", DocumentName = "bdce0f9c-183e-41ab-bca4-5104d887cccc.jpg", OriginalDocumentName = "OriginalDoc3", DocumentDate = 20230103 }
                };

                _dbContext.Documents.AddRange(documents);
                _dbContext.SaveChanges();
            }
        }

        [Fact]
        public void GetDocuments_ReturnsCorrectDocuments_Paginated()
        {
            // Arrange
            int page = 1;
            int pageSize = 2;

            // Act
            var result = _documentService.GetDocuments(page, pageSize);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("bdce0f9c-183e-41ab-bca4-5104d887aaaa.jpg", result.First().DocumentName);
            Assert.Equal("bdce0f9c-183e-41ab-bca4-5104d887bbbb.jpg", result.Last().DocumentName);
        }

        [Fact]
        public void GetTotalDocuments_ReturnsCorrectCount()
        {
            // Act
            var result = _documentService.GetTotalDocuments();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void GetDocuments_ReturnsEmptyList_WhenNoDocumentsExist()
        {
            // Arrange
            _dbContext.Documents.RemoveRange(_dbContext.Documents);
            _dbContext.SaveChanges();

            int page = 1;
            int pageSize = 2;

            // Act
            var result = _documentService.GetDocuments(page, pageSize);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetDocuments_ReturnsCorrectDocuments_WhenPageIsOutOfRange()
        {
            // Arrange
            int page = 2;
            int pageSize = 3;

            // Act
            var result = _documentService.GetDocuments(page, pageSize);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetTotalDocuments_ReturnsZero_WhenNoDocumentsExist()
        {
            // Arrange
            _dbContext.Documents.RemoveRange(_dbContext.Documents);
            _dbContext.SaveChanges();

            // Act
            var result = _documentService.GetTotalDocuments();

            // Assert
            Assert.Equal(0, result);
        }
    }
}

