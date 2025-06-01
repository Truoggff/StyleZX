using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleZX.Models.ViewModel
{
    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }
}