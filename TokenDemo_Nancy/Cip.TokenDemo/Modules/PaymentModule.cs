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

                var transaction = new CIP.Transaction()
                {
                    Token = payment.CipToken,
                    Amount = payment.Amount,
                    TransactionType = payment.TransactionType
                };

                var result = CIP.Token.RunTransaction(transaction);

                return View["receipt", result];
            };

            Get["/{param1}/{param2}"] = parameters =>
            {
                return new RedirectResponse("/");
            };
        }
    }
}