using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using CWDocMgr.Controllers;
using CWDocMgr.Services;
using CWDocMgr.Models;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CWDocMgr.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CWDocMgrTests.ControllerTests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly ApplicationDbContext _inMemoryApplicationDbContext;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public HomeControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _inMemoryApplicationDbContext = new ApplicationDbContext(null, options);


            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockConfiguration = new Mock<IConfiguration>();

            _controller = new HomeController(_mockLogger.Object, 
                _mockDocumentService.Object,
                _mockConfiguration.Object,
                _inMemoryApplicationDbContext);
        }

        [Fact]
        public void Index_UserNotAuthenticated_RedirectsToLogin()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/Identity/Account/Login", redirectResult.Url);
        }

        [Fact]
        public void Index_UserAuthenticated_ReturnsViewWithModel()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "testuser") }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockDocumentService.Setup(s => s.GetTotalDocuments()).Returns(100);
            _mockDocumentService.Setup(s => s.GetDocuments(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<DocumentModel>());

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IndexViewModel>(viewResult.Model);
            Assert.Equal(1, model.PageNumber);
            Assert.Equal(11, model.TotalPages);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        //[Fact]
        //public void Error_ReturnsViewWithErrorViewModel()
        //{
        //    // Act
        //    var result = _controller.Error();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        //    Assert.NotNull(model.RequestId);
        //}

        //private IConfiguration CreateConfiguration()
        //{
        //    // https://medium.com/@TheLe0/mocking-your-appsettings-in-unit-tests-on-net-cb057de7db64
        //    var inMemorySettings = new Dictionary<string, string> {
        //        {"SQLiteDataContext", "CWDocs"},
        //        {"SQLiteDbPath", "C:\\Work\\A_My_Websites\\CWDocs\\UnitTests\\CWDocs.db"},
        //        {"DownloadFilePath", "C:\\Temp"},

        //        //{"SectionName:SomeKey", "SectionValue"},
        //    };

        //    IConfiguration configuration = new ConfigurationBuilder()
        //        .AddInMemoryCollection(inMemorySettings)
        //        .Build();

        //    return configuration;
        //}

    }
}
