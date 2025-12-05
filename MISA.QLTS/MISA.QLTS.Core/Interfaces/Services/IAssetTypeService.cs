using MISA.QLTS.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Services
{
    public interface IAssetTypeService
    {
        Task<IEnumerable<AssetTypeDto>> GetAllAsync();
        Task<AssetTypeDto?> GetByCodeAsync(string code);
    }
}
