using CIP.TokenDemo.Models;
using Nancy;
using Nancy.Json;
using Nancy.ModelBinding;
using Nancy.Responses;
using System;

namespace CIP.TokenDemo.Modules
{
    public class PaymentModule : NancyModule
    {
        public static string Environment = "staging";

        public PaymentModule()
        {
            Get["/"] = parameters =>
            {
                var model = new Payment() { Environment = Environment };

                return View["payment", model];
            };

            Get["/payment"] = parameters =>
            {
                var model = new Payment() { Environment = Environment };

                return View["payment", model];
            };

            Get["/payment/{environment}"] = parameters =>
            {
                var model = new Payment();

                model.Environment = parameters.environment;

                return View["payment", model];
            };

            Post["/payment"] = parameters =>
            {
                var payment = this.Bind<Payment>();

                /* This is the Server Side CIP integration */
                CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";
                //CIP.Token.ApiKey = "4d01abf522ce4025929b4515f3017f74";

                //CIP.Token.Url = "https://psl.chargeitpro.com/token/transaction.json";
                CIP.Token.Url = "http://localhost:57192/token/transaction.json";

                var transaction = new CIP.Transaction()
                {
                    Token = payment.CipToken,
                    ReferenceId = (payment.ReferenceId != null) ? payment.ReferenceId : Guid.NewGuid().ToString(),
                    Amount = payment.Amount,
                    TransactionType = payment.TransactionType,
                    Invoice = "123ABC",
                    PONumber = "Test Po Number",
                    OrderId = "Test OrderId",
                    Description = "Test Description",
                    Email = payment.Email,
                    BillingAddress = new BillingAddress()
                    { 
                        FirstName = payment.BillingFirstName,
                        LastName = payment.BillingLastName,
                        Company = payment.BillingCompany,
                        Street = payment.BillingStreet,
                        Street2 = payment.BillingStreet2,
                        City = payment.BillingCity,
                        State = payment.BillingState,
                        Zip = payment.BillingZip,
                        Country = payment.BillingCountry,
                        Phone = payment.BillingPhone,
                        Email = payment.BillingEmail
                    },
                    ShippingAddress = new ShippingAddress()
                    {
                        FirstName = payment.ShippingFirstName,
                        LastName = payment.ShippingLastName,
                        Company = payment.ShippingCompany,
                        Street = payment.ShippingStreet,
                        Street2 = payment.ShippingStreet2,
                        City = payment.ShippingCity,
                        State = payment.ShippingState,
                        Zip = payment.ShippingZip,
                        Country = payment.ShippingCountry,
                        Phone = payment.ShippingPhone
                    },
                    Comments = payment.Comments
                };

                var response = CIP.Token.RunTransaction(transaction);

                if (!response.Success)
                {
                    var model = new Payment() { Error = response.Error.Message };

                    ViewBag.Referrer = "payment";

                    return View["payment", model];
                }
                else
                {
                    ViewBag.Referrer = "payment";

                    return View["receipt", response.Result];
                }
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
                //CIP.Token.Url = "https://psl.chargeitpro.com/token/transaction.json";
                CIP.Token.Url = "http://localhost:57192/token/transaction.json";

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

            /* ACH Payments*/

            Get["/achpayment"] = parameters =>
            {
                var model = new Payment() { Environment = Environment };

                return View["achpayment", model];
            };

            Post["/achpayment"] = parameters =>
            {
                var payment = this.Bind<Payment>();

                /* This is the Server Side CIP integration */
                CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";
                //CIP.Token.ApiKey = "4d01abf522ce4025929b4515f3017f74";

                CIP.Token.Url = "https://psl.chargeitpro.com/token/transaction.json";
                //CIP.Token.Url = "http://localhost:57192/token/transaction.json";

                var transaction = new CIP.Transaction()
                {
                    Token = payment.CipToken,
                    ReferenceId = Guid.NewGuid().ToString(),
                    Amount = payment.Amount,
                    TransactionType = "ACHSale",
                    Invoice = "123ABC",
                    PONumber = "Test Po Number",
                    OrderId = "Test OrderId",
                    Description = "Test Description",
                    Email = payment.Email,
                    BillingAddress = new BillingAddress()
                    {
                        FirstName = payment.BillingFirstName,
                        LastName = payment.BillingLastName,
                        Company = payment.BillingCompany,
                        Street = payment.BillingStreet,
                        Street2 = payment.BillingStreet2,
                        City = payment.BillingCity,
                        State = payment.BillingState,
                        Zip = payment.BillingZip,
                        Country = payment.BillingCountry,
                        Phone = payment.BillingPhone,
                        Email = payment.BillingEmail
                    },
                    ShippingAddress = new ShippingAddress()
                    {
                        FirstName = payment.ShippingFirstName,
                        LastName = payment.ShippingLastName,
                        Company = payment.ShippingCompany,
                        Street = payment.ShippingStreet,
                        Street2 = payment.ShippingStreet2,
                        City = payment.ShippingCity,
                        State = payment.ShippingState,
                        Zip = payment.ShippingZip,
                        Country = payment.ShippingCountry,
                        Phone = payment.ShippingPhone
                    },
                    Comments = payment.Comments
                };

                var response = CIP.Token.RunTransaction(transaction);

                if (!response.Success)
                {
                    var model = new Payment() { Error = response.Error.Message };

                    ViewBag.Referrer = "achpayment";

                    return View["payment", model];
                }
                else
                {
                    ViewBag.Referrer = "payment";

                    return View["receipt", response.Result];
                }
            };


            Get["/{param1}/{param2}"] = parameters =>
            {
                return new RedirectResponse("/");
            };

            /* IE9 Tests */
            Get["/ie_test"] = parameters =>
            {
                return View["ie_test"];
            };

            Post["/ie_test"] = parameters =>
            {
                var payment = this.Bind<Payment>();

                return View["ie_test"];
            };

            Get["/ie_test2"] = parameters =>
            {
                return View["ie_test2"];
            };


            Get["/ie_test3"] = parameters =>
            {
                return View["ie_test2"];
            };

            Get["/paymenttest"] = parameters =>
            {
                return View["paymenttest"];
            };

            Post["/paymenttest"] = parameters =>
            {
                var payment = this.Bind<Payment>();

                CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";

                CIP.Token.Url = "https://psl.chargeitpro.com/token/transaction.json";

                var transaction = new CIP.Transaction()
                {
                    Token = payment.CipToken,
                    Amount = 5.00,
                    TransactionType = "CreditSale"
                };

                var response = CIP.Token.RunTransaction(transaction);

                var serializer = new JavaScriptSerializer();

                var result = serializer.Serialize(response.Result);

                return View["paymenttest", result];
            };
        }
    }
}