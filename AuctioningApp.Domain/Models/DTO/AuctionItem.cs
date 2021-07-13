using AuctioningApp.Domain.Models.DBM;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctioningApp.Domain.Models.DTO
{
    public class AuctionItem
    {
        public int ID { get; set; }
        public User TopBidder { get; set; }
        public Product Product { get; set; }
        public string Description { get; set; }
        public DateTime StartOfAuction { get; set; }
        public DateTime EndOfAuction { get; set; }
        public List<Bid> Bids { get; set; }
        public Bid TopBid { get; set; }
        public Boolean Highlighted { get; set; }
        public int StartingPrice { get; set; }
        public User CreatedBy { get; set; }
    }
}
