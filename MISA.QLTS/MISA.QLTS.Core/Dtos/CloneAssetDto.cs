using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos
{
    public class CloneAssetDto
    {
        public string AssetCode { get; set; }
        public string NewAssetCode { get; set; }
        public AssetResponseDto ClonedAsset { get; set; } = new AssetResponseDto();
    }
}
