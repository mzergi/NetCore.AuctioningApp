using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctioningApp.API.WebModels
{
    public class WebCategory
    {
        public string Name { get; set; }
        public string ParentCategoryId { get; set; }
    }
}
