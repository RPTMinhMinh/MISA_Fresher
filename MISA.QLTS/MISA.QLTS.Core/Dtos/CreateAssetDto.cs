using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    /// <summary>
    /// Data Transfer Object cho việc tạo thông tin tài sản
    /// </summary>
    public class CreateAssetDto
    {
        [Required(ErrorMessage = "Tên tài sản là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên tài sản không được vượt quá 100 ký tự")]
        public string AssetName { get; set; }

        [Required(ErrorMessage = "Mã bộ phận sử dụng là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Mã bộ phận sử dụng không được vượt quá 20 ký tự")]
        public string DepartmentCode { get; set; }

        [Required(ErrorMessage = "Mã loại tài sản là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Mã loại tài sản không được vượt quá 20 ký tự")]
        public string AssetTypeCode { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Nguyên giá là bắt buộc")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Nguyên giá phải lớn hơn 0")]
        public decimal OriginalPrice { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}
