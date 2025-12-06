using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    public class AssetResponseDto
    {
        public Guid AssetId { get; set; } 
        public string AssetCode { get; set; } 
        public string AssetName { get; set; } 
        public string DepartmentCode { get; set; } 
        public string DepartmentName { get; set; } 
        public string AssetTypeCode { get; set; } 
        public string AssetTypeName { get; set; } 
        public DateTime CreatedDate { get; set; }
        public decimal UseYear { get; set; }
        public decimal UsefulLife { get; set; }
        public decimal TrackingStartYear { get; set; }
        public decimal DecreciationRate { get; set; }
        public decimal Quantity { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal AnnualDecreciation { get; set; }
    }
}
