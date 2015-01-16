using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CIP;
using TokenClientDemoMVC.Models;

namespace TokenClientDemoMVC.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Payment(Payment input)
        {
            /* Set your Private Key */
            CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";

            /* Create a Transaction */
            var transaction = new CIP.Transaction()
            {
                Token = input.CipToken,
                Amount = input.Amount,
                TransactionType = input.TransactionType                
            };

            /* Process the transaction */
            TransactionResult result = CIP.Token.RunTransaction(transaction);

            /* Save the result to your database and/or render the result values to your receipt view */

            var model = new Result()
            {
                ApprovalNumberResult = result.ApprovalNumberResult,
                AmountApproved = result.AmountApproved,
                UniqueTransID = result.UniqueTransId,
                ResultStatus = result.ResultStatus
            };

            return View("Receipt", model);
        }


    }
}
