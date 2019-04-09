using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wapicbot.Services
{
    [Serializable]
    public class Bouquet
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public double Price { get; set; }

        public string FlowerCategory { get; set; }
    }
}