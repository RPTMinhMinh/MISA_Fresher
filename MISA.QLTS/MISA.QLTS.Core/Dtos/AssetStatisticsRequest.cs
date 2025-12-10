using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    /// <summary>
    /// Data Transfer Object cho việc lọc thông tin thống kê tài sản
    /// </summary>
    public class AssetStatisticsRequest
    {
        public string? SearchKeyword { get; set; }
        public string? DepartmentCode { get; set; }
        public string? AssetTypeCode { get; set; }
    }
}
