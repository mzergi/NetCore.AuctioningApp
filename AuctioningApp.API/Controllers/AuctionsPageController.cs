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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AuctioningApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AuctionsPageController : ControllerBase
    {
        private readonly IAuctionsService auctionsService;

        private readonly ICategoriesService categoriesService;

        private readonly AuctionsHub auctionsHub;

        private readonly ProductService productService;

        private readonly IWebHostEnvironment hostEnvironment;

        public AuctionsPageController(IAuctionsService auctionsService, ICategoriesService categoriesService,
            AuctionsHub auctionsHub, ProductService productService, IWebHostEnvironment hostEnvironment)
        {
            this.auctionsService = auctionsService;
            this.categoriesService = categoriesService;
            this.auctionsHub = auctionsHub;
            this.productService = productService;
            this.hostEnvironment = hostEnvironment;
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
            try
            {
                return await auctionsService.GetAuction(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        [HttpPatch("auctions/{id}")]
        public async Task<AuctionItem> PostBidOnAuction([FromBody] Bid bid)
        {
            try
            {
                var result = await auctionsService.PostBidOnAuction(bid);

                await auctionsHub.SendBid(bid);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
            try
            {
                return await auctionsService.GetAuctionsOfCategory(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        [HttpGet("auctions/followed-by/{id}")]
        public async Task<List<AuctionItem>> GetAuctionsOfUser(int id)
        {
            try
            {
                return await auctionsService.GetFollowedAuctionsOfUser(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        [HttpPost("auctions/create")]
        public async Task<ActionResult<Auction>> createAuction([FromBody] AuctionCreateDto auction)
        {
            try
            {
                Auction a = new Auction()
                {
                    ProductID = auction.ProductID,
                    Description = auction.Description,
                    StartOfAuction = auction.StartOfAuction,
                    EndOfAuction = auction.EndOfAuction,
                    StartingPrice = auction.StartingPrice,
                    Highlighted = auction.Highlighted,
                    CreatedById = auction.CreatedById,
                    ImageUrl = auction.ImageName
                };
                
                var created = await auctionsService.PostAuction(a);

                var dto = auctionsService.ConvertAuctionToAuctionItem(created);

                await this.auctionsHub.CreatedAuction(dto);

                return created;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        [HttpPost("auctions/image")]
        public async Task<IActionResult> SaveImage([FromForm] IFormFile imageFile)
        {
            try
            {
                var now = DateTime.Now.Ticks.ToString();
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", String.Concat(now, imageFile.FileName));

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                return Ok($"{String.Concat(now, imageFile.FileName)}");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

                await this.auctionsHub.UpdatedAuction();

                return Ok(dto);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("auctions/created-by/{id}")]
        public async Task<IActionResult> getAuctionsCreatedByUser(int id)
        {
            try
            {
                var auctions = await this.auctionsService.GetAuctionsCreatedByUser(id);

                return Ok(auctions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("products/category/{id}")]
        public async Task<IActionResult> getProductsOfCategory(int id)
        {
            try
            {
                var products = await this.productService.GetAllProducts();

                var productsOfCategory = products.Where(p => p.CategoryID == id).ToList();

                return Ok(productsOfCategory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("products")]
        public async Task<IActionResult> PostProduct([FromBody] ProductCreateDto product)
        {
            try
            {
                Product toCreate = new Product()
                {
                    Name = product.Name,
                    CategoryID = product.CategoryID
                };
                var created = await this.productService.CreateProduct(toCreate);

                return Ok(created);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        public class AmountDto
        {
            public string Amount { get; set; }
        }
        [HttpPut("user/{id}/wallet")]
        public async Task<IActionResult> UpdateWallet(int id, [FromBody] AmountDto amount)
        {
            var toAdd = Double.Parse(amount.Amount);
            try
            {
                var updated = await this.auctionsService.AddCashToUser(id, toAdd);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("auctions/{id}")]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            try
            {
                var deleted = await this.auctionsService.RemoveAuction(id);
                await this.auctionsHub.DeletedAuction();
                return Ok(this.auctionsService.ConvertAuctionToAuctionItem(deleted));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
