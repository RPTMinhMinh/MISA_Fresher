using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    /// <summary>
    /// Data Transfer Object cho thông tin thống kê tài sản
    /// </summary>
    public class AssetStatisticsDto
    {
        public decimal TotalQuantity { get; set; }
        public decimal TotalOriginalPrice { get; set; }
        public decimal TotalAnnualDecreciation { get; set; }
        public decimal TotalRemainingValue { get; set; }
    }
}
