using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Aggregates;

namespace ClinicService.Domain.Repositories;

public interface IDoctorRepository : IRepository<Doctor, Guid>
{
    Task<IReadOnlyList<Doctor>> GetAllActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Doctor>> GetByClinicAsync(Guid clinicId, CancellationToken ct = default);
    Task<IReadOnlyList<Doctor>> GetByServiceAsync(Guid clinicId, Guid serviceId, CancellationToken ct = default);
}
