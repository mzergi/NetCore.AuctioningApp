using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuctioningApp.Domain.Models.DBM
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public string ImagePath { get; set; }
        public int CategoryID { get; set; }
        [JsonIgnore]
        public List<Auction> Present_In { get; set; }
    }
}
