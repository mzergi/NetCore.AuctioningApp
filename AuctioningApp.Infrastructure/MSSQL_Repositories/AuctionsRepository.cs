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
    public class AuctionsRepository : IAuctionsRepository
    {
        private readonly MSSQL_Context db;

        public AuctionsRepository(MSSQL_Context db)
        {
            this.db = db;

            db.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public async void DeleteAuction(int id)
        {
            var auction = await db.Auctions.FirstOrDefaultAsync(a => a.ID == id);

            db.Auctions.Remove(auction);

            await db.SaveChangesAsync();
        }

        public async Task<List<Auction>> FindAuctionsByName(string query)
        {
            var auctions = await db.Auctions
                .Include(i => i.Product)
                .ThenInclude(i => i.Category)
                .Include(i => i.TopBidder)
                .Include(i => i.Bids)
                .Include(i => i.CreatedBy)
                .Select(a => a)
                .Where(a => a.Product.Name.Contains(query))
                .ToListAsync();
            var q = query;
            return auctions;
        }

        public async Task<List<Auction>> GetAllAuctions()
        {
            var auctions = await db.Auctions
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.TopBidder)
                .Include(i => i.Bids)
                .Include(i => i.CreatedBy)
                .Select(a => a)
                .AsSplitQuery()
                .OrderBy(a => a.ID)
                .ToListAsync();

            return auctions;
        }

        public async Task<Auction> GetAuction(int id)
        {
            var auction = await db.Auctions
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.TopBidder)
                .Include(i => i.Bids)
                .Include(i => i.CreatedBy)
                .OrderBy(a => a.ID)
                .FirstOrDefaultAsync(a => a.ID == id);

            return auction;
        }

        public async Task<List<Auction>> GetAuctionsOfCategory(Category category)
        {
            var auctions = await db.Auctions
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.TopBidder)
                .Include(i => i.Bids)
                .Include(i => i.CreatedBy)
                .Select(a => a)
                .Where(a => a.Product.Category == category)
                .AsSplitQuery()
                .OrderBy(a => a.ID)
                .ToListAsync();

            return auctions;
        }

        public async Task<List<Auction>> GetHighlightedAuctions(Boolean highlighted)
        {
            return await db.Auctions
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.TopBidder)
                .Include(i => i.Bids)
                .Include(i => i.CreatedBy)
                .Select(a => a)
                .Where(a => a.Highlighted == highlighted)
                .AsSplitQuery()
                .OrderBy(a => a.ID)
                .ToListAsync();
        }

        public async Task<Auction> HighlightAuction(int id, Boolean val)
        {
            var auction = await db.Auctions.FirstOrDefaultAsync(s => s.ID == id);
            //add later: if auction.User.HighlightsLeft > 0, AFTER: auction.User.HighlightsLeft --
            auction.Highlighted = val;

            await db.SaveChangesAsync();

            return auction;
        }

        public async Task<Auction> PostAuction(Auction a)
        {
            using (var tran = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                await db.Auctions.AddAsync(a);

                await db.SaveChangesAsync();

                return a;
            }
        }

        public async Task<Auction> UpdateAuction(int id, Auction auction)
        {
            var toUpdate = await this.GetAuction(id);

            toUpdate.Description = auction.Description;
            toUpdate.EndOfAuction = auction.EndOfAuction;
            toUpdate.StartOfAuction = auction.StartOfAuction;
            toUpdate.StartingPrice = auction.StartingPrice;
            toUpdate.Highlighted = auction.Highlighted;
            toUpdate.Product = auction.Product;
            toUpdate.ProductID = auction.ProductID;
            toUpdate.CreatedBy = auction.CreatedBy;
            toUpdate.CreatedById = auction.CreatedById;

            await this.db.SaveChangesAsync();

            return toUpdate;
        }

        public async Task<List<Auction>> GetAuctionsCreatedByUser(int id)
        {
            return await this.db.Auctions
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.TopBidder)
                .Include(i => i.Bids)
                .Include(i => i.CreatedBy)
                .Select(a => a).Where(a => a.CreatedById == id).ToListAsync();
        }
    }
}
