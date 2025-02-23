using AutoMapper;
using CWDocMgrApp.Services;
using DocMgrLib.Data;
using DocMgrLib.Extensions;
using DocMgrLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace CWDocMgrApp
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            base.OnStartup(e);

            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
            services.AddSingleton(Configuration);

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlite());
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                      .EnableSensitiveDataLogging()
                      .LogTo(Console.WriteLine, LogLevel.Information));

            services.AddDocMgrLibAutoMapper();

            services.AddLogging(configure => configure
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug)
                );

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<MainWindow>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IOCRService, OCRService>();
            services.AddTransient<IUserService, UserService>();
            ServiceProvider = services.BuildServiceProvider();

            MainWindow mainWindow = ServiceProvider.GetService<MainWindow>();
            if (mainWindow != null)
            {
                mainWindow.Show();
            }
        }
    }
}
