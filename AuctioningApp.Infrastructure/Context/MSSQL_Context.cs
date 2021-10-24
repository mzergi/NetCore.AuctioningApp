using AuctioningApp.Domain.Models.DBM;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AuctioningApp.Infrastructure.Context
{
    //Database context
    public class MSSQL_Context : DbContext
    {
        public DbSet<Auction> Auctions { get; set; }

        public DbSet<Bid> Bids { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public MSSQL_Context(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .ToTable("categories");
            modelBuilder.Entity<Category>()
                .HasKey(c => c.ID);
            modelBuilder.Entity<Category>()
                .Property(c => c.Name).HasMaxLength(50).IsRequired(required: true).IsUnicode(unicode: true);
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentCategoryID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Category>()
                .HasData(new[]
                {
                    new Category() {ID = 1,
                                    Name = "Electronics",
                                    ParentCategoryID = null
                    },
                    new Category()
                    {
                                    ID = 2,
                                    Name = "Jewellery",
                                    ParentCategoryID = null
                    },
                    new Category()
                    {
                                    ID = 3,
                                    Name = "Laptops",
                                    ParentCategoryID = 1
                    },
                    new Category()
                    {
                                    ID = 4,
                                    Name = "PC parts",
                                    ParentCategoryID = 1
                    },
                    new Category()
                    {
                                    ID = 5,
                                    Name = "Televisions",
                                    ParentCategoryID = 1
                    },
                    new Category()
                    {
                                    ID = 6,
                                    Name = "Watches",
                                    ParentCategoryID = 2
                    },
                    new Category()
                    {
                                    ID = 7,
                                    Name = "Necklaces",
                                    ParentCategoryID = 2
                    },
                    new Category()
                    {
                                    ID = 8,
                                    Name = "Bracelets",
                                    ParentCategoryID = 2
                    },
                    new Category()
                    {
                                    ID = 9,
                                    Name = "Rings",
                                    ParentCategoryID = 2
                    },
                    new Category()
                    {
                                    ID = 10,
                                    Name = "Furniture",
                                    ParentCategoryID = null
                    },
                    new Category()
                    {
                                    ID = 11,
                                    Name = "Phones",
                                    ParentCategoryID = 1
                    },
                });

            modelBuilder.Entity<Product>()
                .ToTable("products");
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ID);
            modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(50).IsRequired(required: true).IsUnicode(unicode: true);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.ProductsOfCategory)
                .HasForeignKey(f => f.CategoryID);
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Present_In)
                .WithOne(a => a.Product)
                .HasForeignKey(f => f.ID);

            modelBuilder.Entity<Product>()
                .HasData(new[]
                {
                    new Product()
                    {
                        ID = 1,
                        Name = "iPhone X",
                        CategoryID = 11
                    },
                    new Product()
                    {
                        ID = 2,
                        Name = "Samsung Galaxy A41",
                        CategoryID = 11
                    },
                    new Product()
                    {
                        ID = 3,
                        Name = "IKEA Jahrmührgangül asztal",
                        CategoryID = 10
                    },
                }
                );


            modelBuilder.Entity<User>()
                .ToTable("users");
            modelBuilder.Entity<User>()
                .HasKey(u => u.ID);
            modelBuilder.Entity<User>()
                .Property(u => u.Email).HasMaxLength(300).IsRequired(required: true).IsUnicode(unicode: true);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bids)
                .WithOne(b => b.Bidder)
                .HasForeignKey(b => b.ID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            using var hmac = new HMACSHA512();
            var pwBytes = Encoding.UTF8.GetBytes("testing");
            modelBuilder.Entity<User>()
                .HasData(new[]
                {
                    new User() {
                        ID = 1, 
                        Balance = 12000000, 
                        Bids = new List<Bid> (), 
                        Email = "testuser@testing.com", 
                        PasswordHash = hmac.ComputeHash(pwBytes),
                        PasswordSalt = hmac.Key,
                        Name = "Test Ingrid",
                        Birth = new System.DateTime(2000,01,01)
                    }
                });

            modelBuilder.Entity<Bid>()
                .ToTable("bids");
            modelBuilder.Entity<Bid>()
                .HasKey(b => b.ID);
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(b => b.AuctionID);
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Bidder)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.BidderID)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Auction>()
                .ToTable("auctions");
            modelBuilder.Entity<Auction>()
                .HasKey(s => s.ID);
            modelBuilder.Entity<Auction>()
                .HasOne(t => t.TopBidder)
                .WithMany(b => b.FollowedAuctions)
                .HasForeignKey(f => f.TopBidderID);
            modelBuilder.Entity<Auction>()
                .HasOne(a => a.Product)
                .WithMany(p => p.Present_In)
                .HasForeignKey(f => f.ProductID);
            modelBuilder.Entity<Auction>()
                .Property(a => a.Description).HasMaxLength(300).IsRequired(required: false).IsUnicode(unicode: true);
            modelBuilder.Entity<Auction>()
                .HasOne(a => a.CreatedBy)
                .WithMany(u => u.CreatedAuctions)
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
