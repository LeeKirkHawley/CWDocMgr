using DocMgrLib.Data;
using DocMgrLib.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace CWDocMgr.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        // temporary until getting Identity situated
        private UserModel? _loggedinUser = new UserModel {
            UserName = "Kirk",
            Pwd = "Admin",
            Role = "test",
            Id = 1
        };

        public UserModel? LoggedInUser
        {
            get => _loggedinUser;
            set => _loggedinUser = value;
        }

        //public static NLog.Logger _logger { get; set; } = LogManager.GetCurrentClassLogger();


        public AccountService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public UserModel Login(string userName, string password)
        {
            //_logger.Info($"Logging in user {userName}");

            UserModel user = _userService.GetAllowedUser(userName);
            if (user == null)
            {
                return null;
            }

            if (user.Pwd != password)
            {
                return null;
            }

            _loggedinUser = user;

            return user;
        }

        public UserModel CreateUser(string userName, string password, string role)
        {

            //password = HashPassword(password)

            UserModel user = _userService.CreateUser(userName, password, role);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        private string HashPassword(string password)
        {
            return password;
        }

    }
}
