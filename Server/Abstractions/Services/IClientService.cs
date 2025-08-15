using WarehouseManager.Shared.Dtos.Client;

namespace WarehouseManager.Server.Abstractions.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientDto>> GetAllAsync();
        Task<IEnumerable<ClientDto>> GetAllFilteredAsync(bool isActive = true);
        Task<ClientDto?> GetByIdAsync(int id);
        Task<ClientDto> CreateAsync(CreateClientDto dto);
        Task<string> UpdateAsync(int id, UpdateClientDto dto);
        Task<string> DeleteAsync(int id);

        //
        Task<string> ArchiveAsync(int id);
        Task<string> UnarchiveAsync(int id);

        Task<bool> CheckNameUniqueAsync(string name);
        Task<bool> IsInUseAsync(int id);

    }
}
