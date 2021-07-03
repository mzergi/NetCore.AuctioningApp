using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctioningApp.API.WebModels
{
    public class HighestBidByUserQuery
    {
        public int userID { get; set; }
        public int auctionID { get; set; }
    }
}
