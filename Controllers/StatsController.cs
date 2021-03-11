using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using covid19tg_scraper.Data;
using covid19tg_scraper.Models;
using covid19tg_scraper.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace covid19tg_scraper.Controllers
{

    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly ILogger<DetailsController> _logger;
        // Constructor 
        public StatsController(ILogger<DetailsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async  Task<Stats> GetStatsAsync()
        {
            if (InfosCovidProvider.Stats == null)
            {
                InfosCovidProvider.Stats =new Stats( await InfosCovidProvider.GetStatAsync());               
            }
            
            return InfosCovidProvider.Stats;

        }




    }
}
