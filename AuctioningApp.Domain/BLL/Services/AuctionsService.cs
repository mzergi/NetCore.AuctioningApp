using AuctioningApp.Domain.BLL.ServiceInterfaces;
using AuctioningApp.Domain.Models.DBM;
using AuctioningApp.Domain.Models.DTO;
using AuctioningApp.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctioningApp.Domain.BLL.Services
{
    public class AuctionsService : IAuctionsService
    {
        private IAuctionsRepository auctionsRepository;
        private IBidsRepository bidsRepository;
        private IUsersRepository usersRepository;
        private ICategoriesRepository categoriesRepository;

        public AuctionsService(IAuctionsRepository auctionsRepository, IBidsRepository bidsRepository, 
            IUsersRepository usersRepository, ICategoriesRepository categoriesRepository)
        {
            this.auctionsRepository = auctionsRepository;
            this.bidsRepository = bidsRepository;
            this.usersRepository = usersRepository;
            this.categoriesRepository = categoriesRepository;
        }

        public AuctionItem ConvertAuctionToAuctionItem(Auction auction)
        {
            Bid highestBid;
            try
            {
                highestBid = FindHighestBid(auction);
            }
            catch(ArgumentException ex)
            {
                highestBid = null;
            }

            AuctionItem item = new AuctionItem()
            {
                ID = auction.ID,
                TopBidder = auction.TopBidder,
                Product = auction.Product,
                Description = auction.Description,
                StartOfAuction = auction.StartOfAuction,
                EndOfAuction = auction.EndOfAuction,
                Bids = auction.Bids,
                TopBid = highestBid,
                Highlighted = auction.Highlighted,
                StartingPrice = auction.StartingPrice,
                CreatedBy = auction.CreatedBy
            };

            return item;
        }

        public double? FindHighestBidValue(Auction auction)
        {
            if (auction.Bids != null)
            {
                if (auction.Bids.Count > 0)
                {
                    double maxprice = auction.Bids[0].BiddedAmount;
                    foreach (var BiddedAmount in auction.Bids.Select(b => b.BiddedAmount))
                    {
                        if (BiddedAmount > maxprice)
                        {
                            maxprice = BiddedAmount;
                        }
                    }
                    return maxprice;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Bid FindHighestBid(Auction auction)
        {
            if (auction.Bids != null)
            {
                if (auction.Bids.Count > 0)
                {
                    Bid maxbid = auction.Bids[0];
                    double maxprice = maxbid.BiddedAmount;
                    foreach (Bid b in auction.Bids)
                    {
                        if (b.BiddedAmount > maxprice)
                        {
                            maxbid = b;
                            maxprice = b.BiddedAmount;
                        }
                    }
                    return maxbid;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<List<AuctionItem>> GetAllAuctions()
        {
            var auctions = await auctionsRepository.GetAllAuctions();
            var auctionitems = auctions.Select(ConvertAuctionToAuctionItem).ToList();

            return auctionitems;
        }

        public async Task<AuctionItem> GetAuction(int id)
        {
            var auction = await auctionsRepository.GetAuction(id);

            if (auction == null)
            {
                throw new ArgumentException("Can't find auction!");
            }

            return ConvertAuctionToAuctionItem(auction);
        }

        public async Task<Bid> GetHighestBidOnAuctionByUser(int auctionID, int userID)
        {
            var auction = await auctionsRepository.GetAuction(auctionID);

            var bids = auction.Bids.Select(b => b).Where(b => b.BidderID == userID).ToList();

            if (bids.Count > 0)
            {
                var bid = bids[0].BiddedAmount;

                foreach (var BiddedAmount in bids.Select(b => b.BiddedAmount)){
                    if (BiddedAmount > bid)
                        bid = BiddedAmount;
                }

                return bids.FirstOrDefault(b => b.BiddedAmount == bid);
            }
            else throw new ArgumentException("Can't find bids by user on given auction!");
        }

        public async Task<List<AuctionItem>> GetHighlightedAuctions(bool highlighted)
        {
            var auctions = await auctionsRepository.GetHighlightedAuctions(highlighted);

            var items = auctions.Select(ConvertAuctionToAuctionItem).ToList();

            return items;
        }

        public async Task<Auction> HighlightAuction(int id, Boolean val)
        {
            var auction = await GetAuction(id);
            if (auction != null)
            {
                var result = await auctionsRepository.HighlightAuction(id, val);

                return result;
            }
            else
            {
                throw new ArgumentException("Auction can't be null!");
            }
        }

        public async Task<Auction> PostAuction(Auction auction)
        {
            if (auction.StartOfAuction > auction.EndOfAuction)
                throw new ArgumentException("Auction can't be started after it is finished!");
            return await auctionsRepository.PostAuction(auction);
        }

        private bool checkValidBid(Bid bid)
        {
            var highestbid = FindHighestBid(bid.Auction);
            var now = DateTime.Now.ToUniversalTime();

            if (bid.BidderID == bid.Auction.CreatedById) 
                return false;

            if (highestbid != null)
            {
                var highestbidvalue = highestbid.BiddedAmount;
                double refund = (highestbid.BidderID == bid.BidderID) ? highestbidvalue : 0;
                if (bid.BiddedAmount > highestbidvalue 
                    && bid.BiddedAmount <= bid.Bidder.Balance + refund 
                    && bid.Auction.StartOfAuction < now 
                    && bid.Auction.EndOfAuction > now
                    && bid.BidderID != highestbid.BidderID)
                {
                    return true;
                }
            }
            else if (bid.BiddedAmount >= bid.Auction.StartingPrice 
                && bid.BiddedAmount <= bid.Bidder.Balance
                && bid.Auction.StartOfAuction <= now
                && bid.Auction.EndOfAuction >= now)
            {
                return true;
            }

            return false;


        }

        public async Task<AuctionItem> PostBidOnAuction(Bid bid)
        {
            Console.WriteLine("bid started");
            var starttime = new Stopwatch();
            starttime.Start();
            bid.Bidder = await usersRepository.GetUserWithoutIncludes(bid.BidderID);

            bid.Auction = await auctionsRepository.GetAuction(bid.AuctionID);
            var highestbid = FindHighestBid(bid.Auction);
            
            if (highestbid != null)
            {
                var highestbidvalue = highestbid.BiddedAmount;
                double refund = (highestbid.BidderID == bid.BidderID) ? highestbidvalue : 0;
                if (checkValidBid(bid))
                {
                    if (highestbid.BidderID != bid.BidderID)
                    {
                        highestbid.Bidder = await usersRepository.GetUser(highestbid.BidderID);
                        await usersRepository.AddCashToUser(highestbid.Bidder, highestbidvalue);
                    }
                    await bidsRepository.PostBid(bid);
                    await usersRepository.WithdrawFromUser(bid.Bidder, bid.BiddedAmount);
                }
            }
            else if(checkValidBid(bid))
            {
                await bidsRepository.PostBid(bid);
                await usersRepository.WithdrawFromUser(bid.Bidder, bid.BiddedAmount);
            }


            else
            {
                throw new ArgumentException($"Invalid bid!");
            }

            starttime.Stop();

            Console.WriteLine("Time: " + starttime.Elapsed);

            return ConvertAuctionToAuctionItem(bid.Auction);
        }

        public async Task<Auction> RemoveAuction(int id)
        {
            var auction = await auctionsRepository.GetAuction(id);

            if(auction.Bids.Count > 0)
            {
                throw new ArgumentException("Can't delete an auction that already has bids!");
            }

            if(auction == null)
            {
                throw new ArgumentException("Can't find auction!");
            }
            else
            {
                auctionsRepository.DeleteAuction(id);
                return auction;
            }
        }

        public async Task<List<AuctionItem>> GetAuctionsOfCategory(Category category)
        {
            if (category == null) throw new ArgumentException("Category must not be null!");

            var list = await auctionsRepository.GetAuctionsOfCategory(category);

            return list.Select(ConvertAuctionToAuctionItem).ToList();
        }



        public async Task<List<AuctionItem>> GetAuctionsLike(string name)
        {
            var list = await auctionsRepository.FindAuctionsByName(name);

            return list.Select(ConvertAuctionToAuctionItem).ToList();
        }

        public async Task<List<AuctionItem>> GetAuctionsOfCategory(int categoryID)
        {
            var category = await categoriesRepository.GetCategory(categoryID);

            return await GetAuctionsOfCategory(category);
        }

        public async Task<List<AuctionItem>> GetFollowedAuctionsOfUser(int id)
        {
            try
            {
                var auctions = await usersRepository.GetFollowedAuctionsByUser(id);

                return auctions.Select(ConvertAuctionToAuctionItem).ToList();
            }

            catch(ArgumentException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public async Task<Auction> UpdateAuction(int id, Auction toUpdate)
        {
            var auction = await auctionsRepository.GetAuction(id);

            return await auctionsRepository.UpdateAuction(id, toUpdate);
        }

        public Auction ConvertAuctionItemToAuction(AuctionItem auction)
        {
            return new Auction()
            {
                Product = auction.Product,
                Description = auction.Description,
                StartOfAuction = auction.StartOfAuction,
                EndOfAuction = auction.EndOfAuction,
                ProductID = auction.Product.ID,
                Highlighted = auction.Highlighted,
                StartingPrice = auction.StartingPrice,
                CreatedBy = auction.CreatedBy,
                CreatedById = auction.CreatedBy.ID
            };
        }

        public async Task<List<AuctionItem>> GetAuctionsCreatedByUser(int id)
        {
            var auctions = await this.auctionsRepository.GetAuctionsCreatedByUser(id);

            return auctions.Select(ConvertAuctionToAuctionItem).ToList();
        }

        public async Task<User> AddCashToUser(int id, double amount)
        {
            var user = await this.usersRepository.GetUser(id);
            await this.usersRepository.AddCashToUser(user, amount);
            return user;
        }
    }
}
