using System;

namespace CIP.TokenDemo.Models
{
    [Serializable]
    public class Payment
    {
        public string CipToken { get; set; }
        public double Amount { get; set; }
        public string TransactionType { get; set; }
    }
}