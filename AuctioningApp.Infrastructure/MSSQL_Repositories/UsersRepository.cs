using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AuctioningApp.Domain.RepositoryInterfaces;
using AuctioningApp.Infrastructure.Context;
using AuctioningApp.Domain.Models.DBM;
using Microsoft.EntityFrameworkCore;

namespace AuctioningApp.Infrastructure.MSSQL_Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly MSSQL_Context db;

        public UsersRepository(MSSQL_Context db)
        {
            this.db = db;
        }

        public async Task<User> AddUser(User user)
        {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                if (user != null)
                {
                    await db.Users.AddAsync(user);

                    await db.SaveChangesAsync();

                    await tran.CommitAsync();

                    return user;
                }
                else
                {
                    throw new ArgumentException("User must not be null!");
                }
            }
        }

        public async void DeleteUser(int id)
        {
            var user = await GetUser(id);

            if (user != null)
            {
                db.Users.Remove(user);

                await db.SaveChangesAsync();
            }
        }

        public async Task<User> FindUserByCredentials(string email)
        {
            var user = await db.Users
                .Include(u => u.FollowedAuctions)
                .Include(u => u.Bids)
                .FirstOrDefaultAsync(u => u.Email.Equals(email));

            return user;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await db.Users
                .Include(u => u.FollowedAuctions)
                .Include(u => u.Bids)
                .AsSplitQuery()
                .OrderBy(a => a.ID)
                .ToListAsync();

            return users;
        }

        public async Task<List<Bid>> GetBidsByUser(int id)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.ID == id);

            var bids = await db.Bids
                .Include(b => b.Bidder)
                .Where(b => b.BidderID == user.ID)
                .AsSplitQuery()
                .OrderBy(a => a.ID)
                .ToListAsync();

            return bids;
        }

        public async Task<List<Auction>> GetFollowedAuctionsByUser(int id)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.ID == id);

            if (user == null) throw new ArgumentException("No such user");

            var auctions = await db.Auctions
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.TopBidder)
                .Include(i => i.Bids)
                .Select(a => a)
                .AsSplitQuery()
                .OrderBy(a => a.ID)
                .Where(a => a.Bids.Any(b => b.BidderID == id))
                .ToListAsync();

            return auctions;

        }

        public async Task<User> GetUser(int id)
        {
            var user = await db.Users
                .Include(u => u.FollowedAuctions)
                .Include(u => u.Bids)
                .AsSplitQuery()
                .OrderBy(a => a.ID)
                .FirstOrDefaultAsync(u => u.ID == id);

            return user;
        }


        public async Task<User> GetUserWithoutIncludes(int id)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.ID == id);
        }

        public async Task<bool> UserExists(string email)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

            if (user == null) 
                return false;

            return true;
        }

        public async Task<User> WithdrawFromUser(User user, double amount)
        {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                user.Balance -= amount;

                db.Users.Update(user);

                await db.SaveChangesAsync();

                tran.Commit();

                return user;
            }
        }

        public async Task<User> AddCashToUser(User user, double amount)
        {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                user.Balance += amount;

                db.Users.Update(user);

                await db.SaveChangesAsync();

                tran.Commit();

                return user;
            }
        }

    }
}
