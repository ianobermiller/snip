using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Snip.Models
{
    public class Stats
    {
        public int TotalUserCount { get; set; }
        public int TotalSnipCount { get; set; }
        public int TotalViewCount { get; set; }
        public double AverageSnipsPerUser { get; set; }
        public double AverageViewsPerSnip { get; set; }
    }
}