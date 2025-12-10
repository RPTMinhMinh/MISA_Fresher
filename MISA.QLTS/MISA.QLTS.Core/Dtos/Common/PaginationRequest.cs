using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos.Common
{
    /// <summary>
    /// Lớp đại diện cho yêu cầu phân trang và lọc dữ liệu
    /// </summary>
    public class PaginationRequest
    {
        /// <summary>
        /// Kích thước trang tối đa được phép
        /// </summary>
        private const int MaxPageSize = 100;

        private int _pageSize = 10;

        /// <summary>
        /// Số trang hiện tại (phải lớn hơn 0)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số lượng bản ghi trên mỗi trang (từ 1 đến 100)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Kích thước trang phải từ 1 đến 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        /// <summary>
        /// Từ khóa tìm kiếm (tối đa 100 ký tự)
        /// </summary>
        [MaxLength(100, ErrorMessage = "Từ khóa tìm kiếm không được vượt quá 100 ký tự")]
        public string? SearchKeyword { get; set; }

        /// <summary>
        /// Mã bộ phận để lọc (tối đa 20 ký tự)
        /// </summary>
        [MaxLength(20, ErrorMessage = "Mã bộ phận không được vượt quá 20 ký tự")]
        public string? DepartmentCode { get; set; }

        /// <summary>
        /// Mã loại tài sản để lọc (tối đa 20 ký tự)
        /// </summary>
        [MaxLength(20, ErrorMessage = "Mã loại tài sản không được vượt quá 20 ký tự")]
        public string? AssetTypeCode { get; set; }
    }
}
