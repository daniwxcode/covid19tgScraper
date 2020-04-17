using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AngleSharp;
using AngleSharp.Html.Parser;
using covid19tg_scraper.Models;
using System.Text.RegularExpressions;
using covid19tg_scraper.Services;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace covid19tg_scraper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Covid19Controller : ControllerBase
    {
        private readonly ILogger<Covid19Controller> _logger;
        // Constructor
        public Covid19Controller(ILogger<Covid19Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<Stats> GetAsync()
        {
            // Load default configuration
            var config = Configuration.Default.WithDefaultLoader();
            // Create a new browsing context
            var context = BrowsingContext.New(config);
            // This is where the HTTP request happens, returns <IDocument> that // we can query later
            var document = await context.OpenAsync("http://covid19.gouv.tg/");
            var stat = new Stats();
            stat.ActiveCases = document.ReadInteger("#active-cases>div>h2");
            stat.Cured = document.ReadInteger("#cured>div>h2");
            stat.Deaths = document.ReadInteger("#deceased>div>h2");

            return stat;
        }
       




    }

}
