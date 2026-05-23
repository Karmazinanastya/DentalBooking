using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Aggregates;
using PatientService.Domain.Repositories;
using PatientService.Infrastructure.Persistence;

namespace PatientService.Infrastructure.Repositories;

internal sealed class PatientRepository(PatientDbContext db) : IPatientRepository
{
    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Patients.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Patient?> GetByChatIdAsync(long chatId, CancellationToken ct = default) =>
        await db.Patients.FirstOrDefaultAsync(p => p.ChatId == chatId, ct);

    public async Task<Patient?> GetByPhoneAsync(string phoneNumber, CancellationToken ct = default) =>
        await db.Patients.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, ct);

    public async Task<Patient?> GetExistingAsync(long chatId, string phoneNumber, CancellationToken ct = default) =>
        await db.Patients.FirstOrDefaultAsync(p => p.ChatId == chatId || p.PhoneNumber == phoneNumber, ct);

    public async Task<bool> ExistsByChatIdAsync(long chatId, CancellationToken ct = default) =>
        await db.Patients.AnyAsync(p => p.ChatId == chatId, ct);

    public async Task AddAsync(Patient entity, CancellationToken ct = default) =>
        await db.Patients.AddAsync(entity, ct);

    public void Update(Patient entity) => db.Patients.Update(entity);

    public void Remove(Patient entity) => db.Patients.Remove(entity);
}
