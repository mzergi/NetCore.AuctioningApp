using AuctioningApp.Domain.Models.DBM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.RepositoryInterfaces
{
    public interface IAuctionsRepository
    {
        public Task<List<Auction>> GetAllAuctions();

        public Task<Auction> GetAuction(int id);

        public Task<Auction> PostAuction(Auction a);

        public Task<Auction> HighlightAuction(int id, Boolean val);

        public Task<List<Auction>> GetHighlightedAuctions(Boolean highlighted);

        public Task<List<Auction>> GetAuctionsOfCategory(Category category);

        public Task<List<Auction>> FindAuctionsByName(string query);

        public void DeleteAuction(int id);

        public Task<Auction> UpdateAuction(int id, Auction auction);
    }
}
