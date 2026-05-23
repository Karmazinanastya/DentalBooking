using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Doctors.Commands.CreateDoctor;

public sealed record CreateDoctorCommand(
    Guid ClinicId,
    string FirstName,
    string LastName,
    string Specialization,
    string? PhotoUrl,
    string? Bio
) : IRequest<Result<Guid>>;
