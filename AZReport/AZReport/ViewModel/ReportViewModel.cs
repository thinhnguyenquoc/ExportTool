using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZReport.ViewModel
{
    public class ReportViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string Note { get; set; }
        public string Duration { get; set; }
        List<Sale> SaleList {get; set;}
    }
}
