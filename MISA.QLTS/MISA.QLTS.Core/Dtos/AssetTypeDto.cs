using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    /// <summary>
    /// Data Transfer Object cho thông tin tài sản
    /// </summary>
    public class AssetTypeDto
    {
        public Guid AssetTypeId { get; set; }
        public string AssetTypeCode { get; set; }
        public string AssetTypeName { get; set; }
        public int UsefulLife { get; set; }
        public decimal RecreciationRate { get; set; }
    }
}
