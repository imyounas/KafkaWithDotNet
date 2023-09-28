using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDomain
{
    public class OrderRequest
    {
   
        public int OrderId { get; set; }
        public string ProductId { get; set; } = "";
        public string CustomerId { get; set; } = "";
        public int Quantity { get; set; }
        public string Description { get; set; } = "";
    }
}
