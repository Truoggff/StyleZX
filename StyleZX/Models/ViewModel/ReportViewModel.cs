using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleZX.Models.ViewModel
{
    public class ReportViewModel
    {
        public int Year { get; set; }
        public List<MonthlyRevenue> MonthlyRevenue { get; set; }
        public List<ProductSales> TopSellingProducts { get; set; }
    }
}