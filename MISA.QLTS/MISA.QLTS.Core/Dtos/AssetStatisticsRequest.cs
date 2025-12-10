using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    public class AssetStatisticsRequest
    {
        public string? SearchKeyword { get; set; }
        public string? DepartmentCode { get; set; }
        public string? AssetTypeCode { get; set; }
    }
}
