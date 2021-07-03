using AuctioningApp.Domain.Models.DBM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.RepositoryInterfaces
{
    public interface IBidsRepository
    {
        public Task<List<Bid>> GetAllBids();

        public Task<Bid> GetBid(int id);

        public Task<Bid> PostBid(Bid bid);

        public void DeleteBidAsync(int id);
    }
}
