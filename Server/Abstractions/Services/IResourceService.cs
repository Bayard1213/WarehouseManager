using WarehouseManager.Shared.Dtos.Resource;

namespace WarehouseManager.Server.Abstractions.Services
{
    public interface IResourceService
    {
        Task<IEnumerable<ResourceDto>> GetAllAsync();
        Task<IEnumerable<ResourceDto>> GetAllFilteredAsync(bool isActive = true);
        Task<ResourceDto?> GetByIdAsync(int id);
        Task<ResourceDto> CreateAsync(CreateResourceDto dto);
        Task<string> UpdateAsync(int id, UpdateResourceDto dto);
        Task<string> DeleteAsync(int id);

        //
        Task<string> ArchiveAsync(int id);
        Task<string> UnarchiveAsync(int id);

        Task<bool> CheckNameUniqueAsync(string name);
        Task<bool> IsInUseAsync(int id);

    }
}
