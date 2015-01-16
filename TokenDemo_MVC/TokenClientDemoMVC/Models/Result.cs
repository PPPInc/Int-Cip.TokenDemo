using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TokenClientDemoMVC.Models
{
    public class Result
    {
        public string UniqueTransID { get; set; }
        public string BatchNumber { get; set; }
        public string ResultMessage { get; set; }
        public string ResultStatus { get; set; }
        public string ApprovalNumberResult { get; set; }
        public string AmountApproved { get; set; }
        public string AmountBalance { get; set; }
        public string AVSResponseCode { get; set; }
        public string AVSResponseText { get; set; }
        public string CVVResponseCode { get; set; }
        public string CVVResponseText { get; set; }
        public Error Error { get; set; }
        public DateTime Date { get; set; }
    }
}