using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Entities
{
    /// <summary>
    /// Lớp thực thể đại diện cho loại tài sản trong hệ thống
    /// </summary>
    public class AssetType
    {
        public Guid AssetTypeId { get; set; }
        public string AssetTypeCode { get; set; }
        public string AssetTypeName { get; set; }
        public int UsefulLife { get; set; }
        public decimal RecreciationRate { get; set; }

    }
}
