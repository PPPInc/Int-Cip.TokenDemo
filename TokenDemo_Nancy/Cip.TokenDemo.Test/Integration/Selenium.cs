using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Cip.TokenDemo.Test
{
    [TestClass]
    public class Selenium
    {
        [TestMethod]
        public void TestMethod1()
        {
            IWebDriver driver = new FirefoxDriver();

            driver.Navigate().GoToUrl("http://cip-token-demo.azurewebsites.net/payment");
            
            IWebElement accountData = driver.FindElement(By.CssSelector("[data-cip='accountdata']"));

            accountData.Clear();
            
            accountData.SendKeys("4000200011112222");

            IWebElement form = driver.FindElement(By.Id("payment-form"));

            form.Submit();

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5000));

            IWebElement uniqueTransId = driver.FindElement(By.Id("unique-trans-id"));
            
            //System.Console.WriteLine("Page title is: " + driver.Title);
            //driver.Quit();
        }
    }
}
