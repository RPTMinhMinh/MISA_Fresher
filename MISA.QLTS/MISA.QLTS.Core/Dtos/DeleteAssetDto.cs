using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    /// <summary>
    /// Data Transfer Object cho việc xóa tài sản
    /// </summary>
    public class DeleteAssetDto
    {
        [Required(ErrorMessage = "Danh sách mã tài sản là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một mã tài sản")]
        public List<string> AssetCodes { get; set; }
    }
}
