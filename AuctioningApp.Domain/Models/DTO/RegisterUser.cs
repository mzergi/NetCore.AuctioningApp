using System;
using System.Collections.Generic;
using System.Text;

namespace AuctioningApp.Domain.Models.DTO
{
    public class RegisterUser
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
    }
}
