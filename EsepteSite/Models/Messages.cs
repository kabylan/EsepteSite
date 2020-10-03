using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsepteSite.Models
{
    public class Messages
    {
        public int Id { get; set; }

        public string ClientBusiness { get; set; }

        public string ClientWants { get; set; }

        public string ClientContacts { get; set; }

        public DateTime Date { get; set; }
    }
}
