using Shared.BuildingBlocks.Persistence;
using PatientService.Domain.Aggregates;

namespace PatientService.Domain.Repositories;

public interface IPatientRepository : IRepository<Patient, Guid>
{
    Task<Patient?> GetByChatIdAsync(long chatId, CancellationToken ct = default);
    Task<Patient?> GetByPhoneAsync(string phoneNumber, CancellationToken ct = default);
    Task<Patient?> GetExistingAsync(long chatId, string phoneNumber, CancellationToken ct = default);
    Task<bool> ExistsByChatIdAsync(long chatId, CancellationToken ct = default);
}
