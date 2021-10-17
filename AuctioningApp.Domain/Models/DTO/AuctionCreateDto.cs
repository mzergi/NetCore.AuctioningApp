using System;
using System.Collections.Generic;
using AuctioningApp.Domain.Models.DBM;
using Microsoft.AspNetCore.Http;

namespace AuctioningApp.Domain.Models.DTO
{
    public class AuctionCreateDto
    {
        public string Description { get; set; }
        public DateTime StartOfAuction { get; set; }
        public DateTime EndOfAuction { get; set; }
        public int ProductID { get; set; }
        public Boolean Highlighted { get; set; }
        public int StartingPrice { get; set; }
        public int CreatedById { get; set; }
        public IFormFile Image { get; set; }
    }
}