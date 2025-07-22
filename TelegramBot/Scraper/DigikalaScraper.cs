using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using TelegramBot.Dto.Product;

namespace TelegramBot.Scraper
{
    public class DigikalaScraper
    {
        public List<ProductDto> ScrapeProducts(string searchQuery)
        {
            var products = new List<ProductDto>();
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            using var driver = new ChromeDriver(options);

            var searchUrl = $"https://www.digikala.com/search/?q={Uri.EscapeDataString(searchQuery)}";
            driver.Navigate().GoToUrl(searchUrl);

            Thread.Sleep(6000); // صبر برای بارگذاری کامل

            var productElements = driver.FindElements(By.CssSelector("div.product-list_ProductList__item__LiiNI"));

            foreach (var element in productElements.Take(10))
            {
                string title = "نامشخص";
                string price = "ناموجود";

                try
                {
                    var titleElement = element.FindElement(By.CssSelector("h3"));
                    title = titleElement.Text;
                }
                catch { }

                try
                {
                    var priceElement = element.FindElement(By.CssSelector("div[data-testid='price-final']"));
                    price = priceElement.Text;
                }
                catch { }

                products.Add(new ProductDto
                {
                    Title = title,
                    Price = price
                });
            }

            return products;
        }
    }
}
