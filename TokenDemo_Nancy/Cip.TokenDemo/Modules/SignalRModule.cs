using CIP.TokenDemo.Models;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using System;

namespace CIP.TokenDemo.Modules
{
    public class SignalRModule : NancyModule
    {
        public static string Environment = "staging";

        public SignalRModule()
        {
            Get["/signalrchat"] = parameters =>
            {
                return View["signalrchat", null];
            };
        }
    }
}