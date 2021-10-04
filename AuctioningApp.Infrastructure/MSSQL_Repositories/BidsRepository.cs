using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AuctioningApp.Domain.RepositoryInterfaces;
using AuctioningApp.Infrastructure.Context;
using AuctioningApp.Domain.Models.DBM;

namespace AuctioningApp.Infrastructure.MSSQL_Repositories
{
    public class BidsRepository : IBidsRepository
    {
        private readonly MSSQL_Context db;

        public BidsRepository(MSSQL_Context db)
        {
            this.db = db;
        }
        public async void DeleteBidAsync(int id)
        {
            var bid = await GetBid(id);

            if(bid != null)
            {
                db.Bids.Remove(bid);

                db.SaveChanges();
            }
        }

        public async Task<List<Bid>> GetAllBids()
        {
            return await db.Bids
                .Include(b => b.Auction)
                .ThenInclude(a => a.Product)
                .ThenInclude(p => p.Category)
                .Include(b => b.Bidder)
                .Select(b => b)
                .AsSplitQuery()
                .OrderBy(b => b.ID)
                .ToListAsync();
        }

        public async Task<Bid> GetBid(int id)
        {
            return await db.Bids
                .Include(b => b.Auction)
                .ThenInclude(a => a.Product)
                .ThenInclude(p => p.Category)
                .Include(b => b.Bidder)
                .AsSplitQuery()
                .OrderBy(b => b.ID)
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<Bid> PostBid(Bid bid)
        {
            if (bid != null)
            {
                using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {

                    await db.Bids.AddAsync(bid);

                    await db.SaveChangesAsync();

                    tran.Commit();

                    return bid;
                }
            }
            else
            {
                throw new ArgumentException("Bid must not be null!");
            }
        }
    }
}
