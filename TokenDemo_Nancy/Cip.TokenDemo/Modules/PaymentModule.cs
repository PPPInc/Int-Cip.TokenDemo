using CIP.TokenDemo.Models;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;

namespace CIP.TokenDemo.Modules
{
    public class PaymentModule : NancyModule
    {
        public PaymentModule()
        {
            Get["/"] = parameters =>
            {
                return View["payment"];

            };

            Get["/payment"] = parameters =>
            {
                return View["payment"];
            };

            Post["/payment"] = parameters =>
            {
                var payment = this.Bind<Payment>();

                /* This is the Server Side CIP integration */
                CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";
                CIP.Token.Url = "https://psl.chargeitpro.com/token/transaction.json";
                //CIP.Token.Url = "http://localhost:57192/token/transaction.json";
                
                var transaction = new CIP.Transaction()
                {
                    Token = payment.CipToken,
                    Amount = payment.Amount,
                    TransactionType = payment.TransactionType
                };

                var response = CIP.Token.RunTransaction(transaction);

                ViewBag.Referrer = "payment";

                return View["receipt", response.Result];
            };

            /* Encrypted Payment Examples */
            Get["/encryptedpayment"] = parameters =>
            {
                return View["encrypted_payment"];
            };

            Post["/encryptedpayment"] = parameters =>
            {
                var payment = this.Bind<Payment>();

                /* This is the Server Side CIP integration */
                CIP.Token.ApiKey = "38646a19091049d59ea46e75847fe88f";
                CIP.Token.Url = "https://psl.chargeitpro.com/token/transaction.json";
                //CIP.Token.Url = "http://localhost:57192/token/transaction.json";

                var transaction = new CIP.Transaction()
                {
                    Token = payment.CipToken,
                    Amount = payment.Amount,
                    TransactionType = payment.TransactionType
                };

                var response = CIP.Token.RunTransaction(transaction);

                ViewBag.Referrer = "encryptedpayment";

                return View["receipt", response.Result];
            };

            /* Swipe Payment Examples */
            Get["/swipepayment"] = parameters =>
            {
                return View["swipe_payment"];
            };

            Post["/swipepayment"] = parameters =>
            {
                var payment = this.Bind<Payment>();

                /* This is the Server Side CIP integration */
                CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";
                CIP.Token.Url = "https://psl.chargeitpro.com/token/transaction.json";
                //CIP.Token.Url = "http://localhost:57192/token/transaction.json";

                var transaction = new CIP.Transaction()
                {
                    Token = payment.CipToken,
                    Amount = payment.Amount,
                    TransactionType = payment.TransactionType,
                    Invoice = "Test Demo Invoice"
                };

                var response = CIP.Token.RunTransaction(transaction);

                ViewBag.Referrer = "swipepayment";

                return View["receipt", response.Result];
            };

            Get["/{param1}/{param2}"] = parameters =>
            {
                return new RedirectResponse("/");
            };
        }
    }
}