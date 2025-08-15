using WarehouseManager.Shared.Dtos.Measure;

namespace WarehouseManager.Server.Abstractions.Services
{
    public interface IMeasureService
    {
        Task<IEnumerable<MeasureDto>> GetAllAsync();
        Task<IEnumerable<MeasureDto>> GetAllFilteredAsync(bool isActive = true);
        Task<MeasureDto?> GetByIdAsync(int id);
        Task<MeasureDto> CreateAsync(CreateMeasureDto dto);
        Task<string> UpdateAsync(int id, UpdateMeasureDto dto);
        Task<string> DeleteAsync(int id);

        //
        Task<string> ArchiveAsync(int id);
        Task<string> UnarchiveAsync(int id);
        Task<bool> CheckNameUniqueAsync(string name);
        Task<bool> IsInUseAsync(int id);

    }
}
