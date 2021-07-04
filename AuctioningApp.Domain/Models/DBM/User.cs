using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuctioningApp.Domain.Models.DBM
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public double Balance { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        [JsonIgnore]
        public List<Bid> Bids { get; set; }
        [JsonIgnore]
        public List<Auction> FollowedAuctions { get; set; }
        [JsonIgnore]
        public List<Auction> CreatedAuctions { get; set; }
    }
}
