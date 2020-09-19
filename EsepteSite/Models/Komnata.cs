using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsepteSite.Models
{
    public class Komnata
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string KomnataType { get; set; }
    }

    public class KomnataJSON
    {
        public string komnataType { get; set; }
    }
}
