using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace covid19tg_scraper.Models
{
     public class Stats
    {
        public uint ActiveCases { get; set; }
        public uint Cured { get; set; }
        public uint Deaths { get; set; }
        public string TimeInfo { get; set; }

        public uint Total { get { return ActiveCases + Cured + Deaths; } }
        public override string ToString()
        {
            return $"\n Actifs= {ActiveCases} -Guéris={Cured} - Décès ={Deaths} - Total ={Total}";
        }
        public Stats()
        {

        }
        public Stats(Stat stat)
        {
            Cured =uint.Parse(stat.Recovered);
            Deaths = uint.Parse(stat.Deaths);
            ActiveCases = uint.Parse(stat.Confirmed)-(Cured+Deaths);
            TimeInfo =stat.Last_updated;
        }
    }
    
    public class Stat
    {
        public string Confirmed{get; set; }
        public string Recovered{get;set; }
        public string Deaths{get;set; }
        public string Last_updated{get;set;}
    }

}
