using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    /// <summary>
    /// Data Transfer Object cho việc cập nhật thông tin tài sản
    /// </summary>
    public class UpdateAssetDto
    {
        [MaxLength(100, ErrorMessage = "Tên tài sản không được vượt quá 100 ký tự")]
        public string? AssetName { get; set; }

        [MaxLength(20, ErrorMessage = "Mã bộ phận không được vượt quá 20 ký tự")]
        public string? DepartmentCode { get; set; }

        [MaxLength(20, ErrorMessage = "Mã loại tài sản không được vượt quá 20 ký tự")]
        public string? AssetTypeCode { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public decimal? Quantity { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "Nguyên giá phải lớn hơn 0")]
        public decimal? OriginalPrice { get; set; }

        public DateTime? CreatedDate { get; set; }

        [Range(0, 100, ErrorMessage = "Tỉ lệ hao mòn phải từ 0 đến 100")]
        public decimal? DecreciationRate { get; set; }
    }
}
