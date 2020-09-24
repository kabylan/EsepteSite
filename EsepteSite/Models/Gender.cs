using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsepteSite.Models
{
    public class Gender
    {
        public string Id { get; set; }
        public string ImageLink { get; set; }
        public string ImagePath { get; set; }
        public string TypeRU { get; set; }
    }

    public class GenderJSON
    {
        public string typeRU { get; set; }
    }
}
