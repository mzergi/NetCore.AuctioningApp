using AuctioningApp.Domain.Models.DBM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.RepositoryInterfaces
{
    public interface IUsersRepository
    {
        public Task<List<User>> GetAllUsers();

        public Task<User> GetUser(int id);

        public Task<List<Bid>> GetBidsByUser(int id);

        public Task<List<Auction>> GetFollowedAuctionsByUser(int id);

        public Task<User> FindUserByCredentials(string email);

        public void DeleteUser(int id);

        public Task<User> AddUser(User user);

        public Task<User> WithdrawFromUser(User user, double amount);

        public Task<User> AddCashToUser(User user, double amount);

        public Task<User> GetUserWithoutIncludes(int id);

        public Task<Boolean> UserExists(string email);
    }
}
