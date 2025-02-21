using DocMgrLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CWDocMgrApp.Services
{
    public interface IAccountService
    {
        public UserModel Login(string userName, string password);
        public UserModel CreateUser(string userName, string password, string role);
        public UserModel LoggedInUser { get; set; }

    }
}
