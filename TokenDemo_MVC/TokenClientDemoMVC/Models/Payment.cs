using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TokenClientDemoMVC.Models
{
    public class Payment
    {
        public string CipToken { get; set; }
        public double Amount { get; set; }
        public string TransactionType { get; set; }
    }
}