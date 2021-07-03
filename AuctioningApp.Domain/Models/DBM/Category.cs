using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AuctioningApp.Domain.Models.DBM
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public Category ParentCategory { get; set; }
        [JsonIgnore]
        public List<Category> Children { get; set; }
        [JsonIgnore]
        public List<Product> ProductsOfCategory { get; set; }
        public int? ParentCategoryID { get; set; }
    }
}
