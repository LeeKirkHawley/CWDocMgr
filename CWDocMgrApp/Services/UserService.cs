using DocMgrLib.Data;
using DocMgrLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWDocMgrApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        //public static NLog.Logger _logger { get; set; } = LogManager.GetCurrentClassLogger();

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserModel GetAllowedUser(string userName)
        {
            UserModel user = _context.Users.Where(user => user.UserName == userName).FirstOrDefault();
            return user;
        }

        public UserModel GetAllowedUser(int userId)
        {
            UserModel user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            return user;
        }

        public UserModel CreateUser(string userName, string password, string role)
        {

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserModel> newuser = _context.Users.Add(new UserModel
            {
                UserName = userName,
                Pwd = password,
                Role = role
            });

            int changes = _context.SaveChanges();

            return newuser.Entity;
        }

        public bool DeleteUser(UserModel user)
        {
            try
            {
                var entity = _context.Users.Remove(user);
                int changes = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

    }
}
