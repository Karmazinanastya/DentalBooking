using MediatR;
using Shared.BuildingBlocks.Common;

namespace ClinicService.Application.Clinics.Queries.GetClinics;

public sealed record GetClinicsQuery(string? City) : IRequest<Result<IReadOnlyList<ClinicDto>>>;
