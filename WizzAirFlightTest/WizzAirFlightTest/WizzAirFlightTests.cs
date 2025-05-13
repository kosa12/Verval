using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const int MAX_PRICE = 150; // Árhatár euróban
        private readonly string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Árfolyam (példa): 1 EUR = 4.9 RON
        private const decimal exchangeRate = 4.9m;

        [SetUp]
        public void Setup()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var service = ChromeDriverService.CreateDefaultService();
            service.LogPath = "chromedriver.log";
            service.EnableVerboseLogging = true;
            driver = new ChromeDriver(service);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            TestContext.WriteLine("Setup kész, a driver elindult.");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
            TestContext.WriteLine("Teszt vége, driver bezárva.");
        }

        [Test]
        public void ShouldHaveAtLeastTwoFlightsNextWeek_TGMtoBUD()
        {
            // Navigálás
            driver.Navigate().GoToUrl("https://wizzair.com");
            TestContext.WriteLine("Navigálás a WizzAir oldalra.");

            // Cookie-k elfogadva
            var rejectCookiesButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("/html/body/div[7]/div[2]/div/div[2]/div[1]/div/div[2]/div/div[2]/button")));
            rejectCookiesButton.Click();
            TestContext.WriteLine("Cookie-k elfogadva.");

            // Kiindulási hely
            var fromInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/main/div/div/div[1]/div[1]/div[1]/div[2]/div/div[2]/div/div[1]/form/div/fieldset[1]/div/div[1]/div/div/input")));
            fromInput.Click();
            fromInput.SendKeys("Tirgu Mures");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[3]/div/div/div[1]/label[2]/small"))).Click();
            TestContext.WriteLine("Kiindulási hely beállítva: Tirgu Mures.");

            // Célállomás
            var toInput = driver.FindElement(By.XPath("/html/body/div[1]/div/main/div/div/div[1]/div[1]/div[1]/div[2]/div/div[2]/div/div[1]/form/div/fieldset[1]/div/div[2]/div/div/input"));
            toInput.Click();
            toInput.SendKeys("Budapest");
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[4]/div/div/div/div/div/label/small"))).Click();
            TestContext.WriteLine("Célállomás beállítva: Budapest.");

            // Dátum
            var nextWeekDate = DateTime.Today.AddDays(7);
            string dateAttr = nextWeekDate.ToString("yyyy-MM-dd");
            TestContext.WriteLine($"Következő hét dátuma: {dateAttr}");

            // Rugalmas dátum
            var flexibleDateButton = driver.FindElement(By.XPath("/html/body/div[1]/div/main/div/div/div[1]/div[1]/div[1]/div[2]/div/div[1]/div[2]/button"));
            flexibleDateButton.Click();
            TestContext.WriteLine("Rugalmas dátum kiválasztva.");

            // Várakozás a járatokra
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[1]/div/main/div/div/div[2]/div/div[3]/div[1]/div[2]/div[2]/div[1]/div[2]/ul/li")));
            TestContext.WriteLine("Járatok betöltése megtörtént.");

            var flightItems = driver.FindElements(By.XPath("/html/body/div[1]/div/main/div/div/div[2]/div/div[3]/div[1]/div[2]/div[2]/div[1]/div[2]/ul/li"));
            var validFlights = flightItems.Where(item =>
            {
                try
                {
                    return item.FindElement(By.ClassName("day-selector__price-wrapper")).Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }).ToList();

            int nrOfFlights = validFlights.Count;
            TestContext.WriteLine($"Találat: {nrOfFlights} járat.");

            // Olcsó járat keresése és screenshot
            bool cheapFlightFound = false;
            foreach (var flight in validFlights)
            {
                try
                {
                    var priceElement = flight.FindElement(By.ClassName("day-selector__price-wrapper"));
                    var priceTextRaw = priceElement.Text.Replace("€", "").Replace("RON", "").Trim();
                    TestContext.WriteLine($"Talált ár szöveg (átalakítva): {priceTextRaw}");

                    if (decimal.TryParse(priceTextRaw, out decimal priceInCurrency))
                    {
                        // Átváltás euróra
                        decimal priceInEuro = priceInCurrency / exchangeRate;
                        TestContext.WriteLine($"Ár lejben: {priceInCurrency}, euróban: {priceInEuro}");

                        if (priceInEuro < MAX_PRICE)
                        {
                            // Olcsó járat megtalálva
                            string screenshotFileName = $"CheapFlight_TGMtoBUD_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                            TakeScreenshot(screenshotFileName);
                            TestContext.WriteLine($"Olcsó járat találva: {priceInEuro} EUR. Képernyőkép mentve: {screenshotFileName}");
                            cheapFlightFound = true;
                            break;
                        }
                    }
                    else
                    {
                        TestContext.WriteLine($"Nem sikerült értelmezni az árat: {priceTextRaw}");
                    }
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Hiba az ár ellenőrzése során: {ex.Message}");
                }
            }

            // Ellenőrzés
            nrOfFlights.Should().BeGreaterThanOrEqualTo(2, "legalább két járatnak kell lennie");
            if (cheapFlightFound)
            {
                TestContext.WriteLine("Olcsó járat megtalálva, képernyőkép mentve az Asztalra.");
            }
            else
            {
                TestContext.WriteLine("Nincs olcsó járat a megadott ár alatt.");
            }
        }

        private void TakeScreenshot(string fileName)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                string fullPath = Path.Combine(desktopPath, fileName);
                screenshot.SaveAsFile(fullPath);
                TestContext.WriteLine($"Képernyőkép mentve: {fullPath}");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Hiba a képernyőkép mentése során: {ex.Message}");
            }
        }
    }
}
