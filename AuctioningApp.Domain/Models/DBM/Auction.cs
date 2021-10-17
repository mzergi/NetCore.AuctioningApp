using System;
using System.Collections.Generic;
using System.Text;

namespace AuctioningApp.Domain.Models.DBM
{
    public class Auction
    {
        public int ID { get; set; }
        public User? TopBidder { get; set; }
        public int? TopBidderID { get; set; }
        public Product Product { get; set; }
        public int ProductID { get; set; }
        public string Description { get; set; }
        public DateTime StartOfAuction { get; set; }
        public DateTime EndOfAuction { get; set; }
        public List<Bid> Bids { get; set; }
        public Boolean Highlighted { get; set; }
        public int StartingPrice { get; set; }
        public User CreatedBy { get; set; }
        public int CreatedById { get; set; }
        public string ImageRoute { get; set; }
    }
}
