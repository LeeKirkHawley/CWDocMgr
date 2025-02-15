using DocMgrLib.Data;
using DocMgrLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // Register the DbContext
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite());

            services.AddSingleton<MainWindow>();
            services.AddTransient<IAccountService, AccountService>();
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
