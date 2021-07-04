using AuctioningApp.Domain.Models.DBM;
using AuctioningApp.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.BLL.ServiceInterfaces
{
    public interface IAuctionsService
    {
        Task<List<AuctionItem>> GetAllAuctions();
        Task<AuctionItem> GetAuction(int id);
        Task<Auction> PostAuction(Auction auction);
        Task<AuctionItem> PostBidOnAuction(Bid bid);
        Task<Auction> HighlightAuction(int id, Boolean val);
        void RemoveAuction(int id);
        AuctionItem ConvertAuctionToAuctionItem(Auction auction);
        Auction ConvertAuctionItemToAuction(AuctionItem auction);
        double? FindHighestBidValue(Auction auction);
        Bid FindHighestBid(Auction auction);
        Task<List<AuctionItem>> GetHighlightedAuctions(Boolean highlighted);
        Task<Bid> GetHighestBidOnAuctionByUser(int auctionID, int userID);
        Task<List<AuctionItem>> GetAuctionsOfCategory(Category category);
        Task<List<AuctionItem>> GetAuctionsOfCategory(int categoryID);
        Task<List<AuctionItem>> GetAuctionsLike(string name);
        Task<List<AuctionItem>> GetFollowedAuctionsOfUser(int id);
        Task<Auction> UpdateAuction(int id, Auction toUpdate);
        Task<List<AuctionItem>> GetAuctionsCreatedByUser(int id);
    }
}
