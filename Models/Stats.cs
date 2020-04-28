﻿using System;
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
        public uint Total { get { return ActiveCases + Cured + Deaths; } }

        public string timeInfo { get;  set; }

        public override string ToString()
        {
            return $"\n Actifs= {ActiveCases} -Guéris={Cured} - Décès ={Deaths} - Total ={Total}";
        }
    }
}
