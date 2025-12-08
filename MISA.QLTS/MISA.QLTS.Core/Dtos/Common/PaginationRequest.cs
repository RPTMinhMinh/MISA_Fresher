using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos.Common
{
    public class PaginationRequest
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Kích thước trang phải từ 1 đến 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        [MaxLength(100, ErrorMessage = "Từ khóa tìm kiếm không được vượt quá 100 ký tự")]
        public string? SearchKeyword { get; set; }

        [MaxLength(20, ErrorMessage = "Mã bộ phận không được vượt quá 20 ký tự")]
        public string? DepartmentCode { get; set; }

        [MaxLength(20, ErrorMessage = "Mã loại tài sản không được vượt quá 20 ký tự")]
        public string? AssetTypeCode { get; set; }
    }
}
