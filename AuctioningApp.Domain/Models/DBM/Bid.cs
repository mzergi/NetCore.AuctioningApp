using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuctioningApp.Domain.Models.DBM
{
    public class Bid
    {
        public int ID { get; set; }
        public User Bidder { get; set; }
        public double BiddedAmount { get; set; }
        [JsonIgnore]
        public Auction Auction { get; set; }
        public int BidderID { get; set; }
        public int AuctionID { get; set; }
        public DateTime bidTime { get; set; }
    }
}
