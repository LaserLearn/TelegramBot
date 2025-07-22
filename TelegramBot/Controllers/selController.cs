using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using TelegramBot.Scraper;

namespace TelegramBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DigikalaController : ControllerBase
    {
        [HttpGet("{query}")]
        public IActionResult GetProducts(string query)
        {
            var scraper = new DigikalaScraper();
            var products = scraper.ScrapeProducts(query);
            return Ok(products);
        }
    }
}




