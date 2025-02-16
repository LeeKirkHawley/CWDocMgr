using AutoMapper;
using DocMgrLib.Data;
using DocMgrLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CWDocMgrApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite());

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

            // If you need to use the mapper globally, you could make it static or inject it somewhere
            // Here's an example of making it globally accessible:
            //YourStaticClass.Mapper = mapper;


            services.AddSingleton<MainWindow>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IOCRService, OCRService>();
            services.AddTransient<IUserService, UserService>();
            ServiceProvider = services.BuildServiceProvider();

            // Initialize your main window or whatever else you need to do
            //MainWindow mainWindow = new MainWindow();
            MainWindow mainWindow = ServiceProvider.GetService<MainWindow>();
            if (mainWindow != null)
            {
                //ServiceProvider.GetRequiredService<IServiceProvider>().InjectProperties(mainWindow);
                mainWindow.Show();
            }

        }
    }

}
