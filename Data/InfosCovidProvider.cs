using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using covid19tg_scraper.Models;
using covid19tg_scraper.Services;
using Newtonsoft.Json;
using RestSharp;

namespace covid19tg_scraper.Data
{
    public static class InfosCovidProvider
    {
        public static bool IsUpdate { get; set; }
        public static List<Details> Details { get; set; }
        public static Timer AutoRefreshTimer { get; set; }
        public static Stats Stats { get; set; }
        public static async void AutoRefresh()
        {
            IsUpdate = false;
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(10);

            AutoRefreshTimer = new Timer(async (e) =>
            {
                var newData = await GetDetailsAsync();
                newData.Reverse();
                foreach (var tmp in newData)
                {
                    if (Details == null) Details = newData;
                    if (Details.All(p => p.Stat.TimeInfo != tmp.Stat.TimeInfo))
                    {
                        Details.Insert(0, tmp);
                        IsUpdate = true;
                    }

                }
                var tmpStats = new Stats(await GetStatAsync());
                if (Stats == null)
                {
                    var client = new RestClient("https://tgcovidinfo.firebaseio.com/Stats.json");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.PATCH);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", JsonConvert.SerializeObject(Stats), ParameterType.RequestBody);
                    await client.ExecuteAsync(request);
                }else 
                if (Stats.TimeInfo != tmpStats.TimeInfo)
                {
                    var client = new RestClient("https://tgcovidinfo.firebaseio.com/Stats.json");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.PATCH);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", JsonConvert.SerializeObject(Stats), ParameterType.RequestBody);
                    await client.ExecuteAsync(request);
                }

                if (IsUpdate)
                {
                    var client = new RestClient("https://tgcovidinfo.firebaseio.com/Stats.json");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.PATCH);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", JsonConvert.SerializeObject(Details), ParameterType.RequestBody);
                    await client.ExecuteAsync(request);
                }



            }, null, startTimeSpan, periodTimeSpan);

        }

        public static async Task<List<Details>> GetDetailsAsync()
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


                int i = 0;
                Stats itemStats = new Stats();
                itemStats.TimeInfo = $"{itemHtmlDetails[0].InnerHtml} à { itemHtmlDetails[1].InnerHtml}";
                itemStats.ActiveCases = itemHtmlDetails[3].InnerHtml.GetInt();
                itemStats.Cured = itemHtmlDetails[4].InnerHtml.GetInt();
                itemStats.Deaths = itemHtmlDetails[5].InnerHtml.GetInt();
                itemDetails.Stat = itemStats;

                //    itemDetails.Histoire = Regex.Replace(itemsections[1].TextContent, @"^[\s\t\n]+|[\s\t\n]+$", "\n");
                itemDetails.Histoire = Regex.Replace(itemsections[1].TextContent, @"<[^>]+>|&nbsp;", " ").Trim();
                itemDetails.Histoire = Regex.Replace(itemsections[1].TextContent, @"U", "  U").Trim();

                itemDetails.Histoire = Regex.Replace(itemDetails.Histoire, @"\s{2,}", " ");
                itemDetails.Histoire = Regex.Replace(itemDetails.Histoire, @"\n{2,}", "\n");
                itemDetails.Histoire = Regex.Replace(itemDetails.Histoire, @"\t{3,}", "\t\t"); ;
                details.Add(itemDetails);

            }

            return details;
        }

        public static async Task<Stat> GetStatAsync()
        {
            Stat details;

            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync("https://coronavirus-tg-api.herokuapp.com/v1/cases");
                details = JsonConvert.DeserializeObject<Stat>(json);
                return details;
            }

        }
    }
}
