using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    public class AssetStatisticsDto
    {
        public decimal TotalQuantity { get; set; }
        public decimal TotalOriginalPrice { get; set; }
        public decimal TotalAnnualDecreciation { get; set; }
        public decimal TotalRemainingValue { get; set; }
    }
}
