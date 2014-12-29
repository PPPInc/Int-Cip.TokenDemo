using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TokenClientDemoMVC.Models
{
    public class Result
    {
        public string AuthCode { get; set; }
        public string AuthAmount { get; set; }
        public string Status { get; set; }
        public string RefNumber { get; set; }
    }
}