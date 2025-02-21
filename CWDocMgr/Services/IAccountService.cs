using DocMgrLib.Models;
using System.Security.Claims;

namespace CWDocMgr.Services
{
    public interface IAccountService
    {
        public UserModel Login(string userName, string password);
        public UserModel CreateUser(string userName, string password, string role);
        public UserModel? LoggedInUser { get; set; }

    }
}
