using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Aggregates;

namespace ClinicService.Domain.Repositories;

public interface IClinicRepository : IRepository<Clinic, Guid>
{
    Task<IReadOnlyList<Clinic>> GetByCityAsync(string city, CancellationToken ct = default);
    Task<IReadOnlyList<Clinic>> GetAllActiveAsync(CancellationToken ct = default);
}
