using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Newtonsoft.Json;
using AuctioningApp.Domain.BLL.ServiceInterfaces;
using AuctioningApp.API.Hubs;
using AuctioningApp.Domain.BLL.Services;
using AuctioningApp.Domain.Models.DBM;
using AuctioningApp.API.WebModels;
using AuctioningApp.Domain.Models.DTO;

namespace AuctioningApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsPageController : ControllerBase
    {
        private readonly IAuctionsService auctionsService;

        private readonly ICategoriesService categoriesService;

        private readonly AuctionsHub auctionsHub;

        private readonly ProductService productService;

        public AuctionsPageController(IAuctionsService auctionsService, ICategoriesService categoriesService,
            AuctionsHub auctionsHub, ProductService productService)
        {
            this.auctionsService = auctionsService;
            this.categoriesService = categoriesService;
            this.auctionsHub = auctionsHub;
            this.productService = productService;
        }
        //api/auctionspage/categories
        [HttpGet("categories")]
        public async Task<List<Category>> GetCategories()
        {
            var categories = await categoriesService.GetAllCategories();

            return categories;
        }
        [HttpGet("categories/{id}")]
        public async Task<Category> GetCategory(int id)
        {
            return await categoriesService.GetCategory(id);
        }
        [HttpPost("categories")]
        public async Task<ActionResult<Category>> PostCategory([FromBody] WebCategory category)
        {

            Category toPost = new Category
            {
                Name = category.Name,
                ParentCategory = null,
            };
            int parentId = int.Parse(category.ParentCategoryId);

            try
            {
                var created = await categoriesService.PostCategory(toPost, category.ParentCategoryId);
                return CreatedAtAction(nameof(GetCategory), new { id = created.ID }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //GET api/auctionspage/auctions
        [HttpGet("auctions")]
        public async Task<List<AuctionItem>> GetAuctionItems()
        {
            return await auctionsService.GetAllAuctions();
        }
        //GET api/auctionspage/auctions/id
        [HttpGet("auctions/{id}")]
        public async Task<AuctionItem> GetAuctionItem(int id)
        {
            return await auctionsService.GetAuction(id);
        }
        [HttpPatch("auctions/{id}")]
        public async Task<AuctionItem> PostBidOnAuction([FromBody] Bid bid)
        {
            var result = await auctionsService.PostBidOnAuction(bid);

            await auctionsHub.SendBid(bid);

            return result;
        }
        [HttpGet("auctions/highlighted")]
        public async Task<List<AuctionItem>> GetHighlightedAuctions()
        {
            return await auctionsService.GetHighlightedAuctions(true);
        }
        [HttpGet("auctions/basic")]
        public async Task<List<AuctionItem>> GetBasicAuctions()
        {
            return await auctionsService.GetHighlightedAuctions(false);
        }
        [HttpGet("auctions/{auctionid}/highestByUser/{userid}")]
        public async Task<Bid> GetHighestBidByUser(int auctionid, int userid)
        {
            try
            {
                return await auctionsService.GetHighestBidOnAuctionByUser(auctionid, userid);
            }
            catch (ArgumentException ex)
            {
                return new Bid() { BiddedAmount = 0, BidderID = userid, AuctionID = auctionid };
            }
        }

        [HttpGet("auctions/search")]
        public async Task<List<AuctionItem>> GetAuctionsBySearch([FromQuery] string query)
        {
            return await auctionsService.GetAuctionsLike(query);
        }

        [HttpGet("auctions/category/{id}")]
        public async Task<List<AuctionItem>> GetAuctionsOfCategory(int id)
        {
            return await auctionsService.GetAuctionsOfCategory(id);
        }
        [HttpGet("auctions/followed-by/{id}")]
        public async Task<List<AuctionItem>> GetAuctionsOfUser(int id)
        {
            return await auctionsService.GetFollowedAuctionsOfUser(id);
        }
        [HttpPost("auctions/create")]
        public async Task<ActionResult<Auction>> createAuction([FromBody] AuctionItem auction)
        {
            Auction a = auctionsService.ConvertAuctionItemToAuction(auction);

            if (await productService.GetProduct(auction.Product.ID) == null)
                await productService.CreateProduct(auction.Product);

            return await auctionsService.PostAuction(a);
        }

        [HttpGet("products")]
        public async Task<List<Product>> getAllProducts()
        {
            return await productService.GetAllProducts();
        }
        [HttpPut("auctions/{id}")]
        public async Task<IActionResult> updateAuction(int id, [FromBody] AuctionItem toUpdate)
        {
            try
            {
                Auction a = auctionsService.ConvertAuctionItemToAuction(toUpdate);

                if (await productService.GetProduct(toUpdate.Product.ID) == null)
                    await productService.CreateProduct(toUpdate.Product);

                var updated = await auctionsService.UpdateAuction(id, a);

                var dto = auctionsService.ConvertAuctionToAuctionItem(updated);

                return Ok(dto);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("auctions/createdBy/{id}")]
        public async Task<IActionResult> getAuctionsCreatedByUser(int id)
        {
            var auctions = await this.auctionsService.GetAuctionsCreatedByUser(id);

            return Ok(auctions);
        }
    }
}
