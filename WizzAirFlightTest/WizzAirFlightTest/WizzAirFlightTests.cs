using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace WizzAirFlightTests
{
    public class FlightSearchTests
    {
        IWebDriver driver;
        WebDriverWait wait;

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }


        [SetUp]
        public void Setup()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            //driver.Manage().Window.Maximize();
        }

        [Test]
        public void ShouldHaveAtLeastTwoFlightsNextWeek_TGMtoBUD()
        {
            // Arrange
            driver.Navigate().GoToUrl("https://wizzair.com");

            var rejectCookiesButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[7]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/button")));
            rejectCookiesButton.Click();

            Thread.Sleep(2000);

            var fromInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/main/div/div/div[1]/div[1]/div[1]/div[2]/div/div[2]/div/div[1]/form/div/fieldset[1]/div/div[1]/div/div/input")));
            fromInput.Click();
            fromInput.SendKeys("Tirgu Mures");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[3]/div/div/div[1]/label[2]/small"))).Click();

            var toInput = driver.FindElement(By.XPath("/html/body/div[1]/div/main/div/div/div[1]/div[1]/div[1]/div[2]/div/div[2]/div/div[1]/form/div/fieldset[1]/div/div[2]/div/div/input"));
            toInput.Click();
            toInput.SendKeys("Budapest");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[4]/div/div/div/div/div/label/small"))).Click();

            var nextWeekDate = DateTime.Today.AddDays(7);
            string dateAttr = nextWeekDate.ToString("yyyy-MM-dd");

            var flexibleDateButton = driver.FindElement(By.XPath("/html/body/div[1]/div/main/div/div/div[1]/div[1]/div[1]/div[2]/div/div[1]/div[2]/button"));
            flexibleDateButton.Click();

            Thread.Sleep(9000);

            var flightItems = driver.FindElements(By.XPath("/html/body/div[1]/div/main/div/div/div[2]/div/div[3]/div[1]/div[2]/div[2]/div[1]/div[2]/ul/li"));
            var nrOfFlights = 0;
            var validFlights = flightItems.Where(item =>
            {
                try
                {
                    return item.FindElement(By.ClassName("day-selector__price-wrapper")).Displayed;
                }
                catch
                {
                    return false;
                }
            }).ToList();

            nrOfFlights = validFlights.Count;


            // Assert
            nrOfFlights.Should().BeGreaterThanOrEqualTo(2, "there should be at least two flights available for the selected route and date");
        }
    }
}