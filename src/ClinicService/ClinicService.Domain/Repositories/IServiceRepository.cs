using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Entities;

namespace ClinicService.Domain.Repositories;

public interface IServiceRepository : IRepository<Service, Guid>
{
    Task<IReadOnlyList<Service>> GetAllActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Service>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<IReadOnlyList<Service>> GetByClinicAsync(Guid clinicId, CancellationToken ct = default);
}
