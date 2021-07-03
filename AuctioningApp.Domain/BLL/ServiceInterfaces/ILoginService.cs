using AuctioningApp.Domain.Models.DBM;
using AuctioningApp.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.BLL.ServiceInterfaces
{
    public interface ILoginService
    {
        public Task<string> Login(LoginInfo user);
        public Boolean CheckPassword(string password, byte[] passwordhash, byte[] salt);
        public string GenerateJWTToken(User user);
        public Task<User> Register(RegisterUser user);
        public void CreatePasswordHash(string password, User user);
    }
}
