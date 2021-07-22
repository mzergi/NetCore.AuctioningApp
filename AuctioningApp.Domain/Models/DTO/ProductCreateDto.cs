using System.Security.Cryptography;
using AuctioningApp.Domain.Models.DBM;

namespace AuctioningApp.Domain.Models.DTO
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public Category Category { get; set; }
        public string ImagePath { get; set; }
        public int CategoryID { get; set; }
    }
}