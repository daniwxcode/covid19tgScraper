using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using covid19tg_scraper.Models;
using covid19tg_scraper.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace covid19tg_scraper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        private readonly ILogger<DetailsController> _logger;
        // Constructor
        public DetailsController(ILogger<DetailsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<InfosCovid> GetDetailsAsync()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync("https://covid19.gouv.tg/situation-au-togo/");
            var details = new List<Details>();
           
            var sections = document.QuerySelectorAll(".ee-loop__item>article>div>div>div");
            string xt = string.Empty;

            foreach (var item in sections.Skip(1))
            {
                var itemDetails = new Details();
                xt += "\n\n";
                var itemsections = item.QuerySelectorAll("section");
                var itemHtmlDetails = itemsections.FirstOrDefault().QuerySelectorAll("h2");
                itemDetails.Date =$"{itemHtmlDetails[0].InnerHtml} à { itemHtmlDetails[1].InnerHtml}";
               
                int i = 0;
                Stats itemStats = new Stats();
                itemStats.ActiveCases = itemHtmlDetails[3].InnerHtml.GetInt();
                itemStats.Cured = itemHtmlDetails[4].InnerHtml.GetInt();
                itemStats.Deaths = itemHtmlDetails[5].InnerHtml.GetInt();
                itemDetails.Stat = itemStats;
                foreach (var history in itemsections[1].QuerySelectorAll("p"))
                {
                    itemDetails.Histoire += $"\n{history.InnerHtml}";
                }
                details.Add(itemDetails);

            }

            return new InfosCovid(details);
        }
    }
}