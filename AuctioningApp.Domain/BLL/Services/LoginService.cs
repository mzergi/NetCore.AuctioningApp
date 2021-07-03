using AuctioningApp.Domain.BLL.ServiceInterfaces;
using AuctioningApp.Domain.Models.DBM;
using AuctioningApp.Domain.Models.DTO;
using AuctioningApp.Domain.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.BLL.Services
{
    //JWT Token generálás Katona Tamás szakdolgozata alapján. Köszönöm a segítséget!
    public class LoginService : ILoginService
    {
        private readonly IUsersRepository usersRepository;

        private readonly IConfiguration configuration;

        public LoginService(IUsersRepository _usersRepository, IConfiguration config)
        {
            usersRepository = _usersRepository;
            configuration = config;
        }

        public async Task<string> Login(LoginInfo userinfo)
        {
            var user = await usersRepository.FindUserByCredentials(userinfo.Email);

            if (user == null)
                throw new Exception("User not found!");
            if (!CheckPassword(userinfo.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Wrong username or password!");

            return GenerateJWTToken(user);
        }

        public async Task<User> Register(RegisterUser register)
        {
            register.Email = register.Email.ToLower();
            if (await usersRepository.UserExists(register.Email))
                throw new Exception("Email already in use!");

            User user = new User
            {
                Email = register.Email,
                Balance = 0,
                Name = register.Name,
                Birth = register.Birth
            };

            CreatePasswordHash(register.Password, user);

            await usersRepository.AddUser(user);

            return user;
        }
        public bool CheckPassword(string password, byte[] passwordhash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var pwBytes = Encoding.UTF8.GetBytes(password);
                var hash = hmac.ComputeHash(pwBytes);
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != passwordhash[i])
                        return false;
                }
            }

            return true;
        }

        public string GenerateJWTToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var conf = configuration.GetSection("AppSettings:Token").Value;
            var key = Encoding.ASCII.GetBytes(conf);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", user.ID.ToString()),
                    new Claim("password", user.PasswordHash.ToString()),
                    new Claim("email", user.Email),
                    new Claim("balance", user.Balance.ToString()),
                    new Claim("name", user.Name),
                    new Claim("birth", user.Birth.ToString()),
                }),
                Expires = DateTime.Now.AddHours(12),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void CreatePasswordHash(string password, User user)
        {
            using var hmac = new HMACSHA512();
            var pwBytes = Encoding.UTF8.GetBytes(password);
            user.PasswordHash = hmac.ComputeHash(pwBytes);
            user.PasswordSalt = hmac.Key;
        }
    }
}
