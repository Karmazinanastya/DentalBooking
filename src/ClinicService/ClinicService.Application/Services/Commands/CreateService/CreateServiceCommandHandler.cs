using MediatR;
using Shared.BuildingBlocks.Common;
using Shared.BuildingBlocks.Persistence;
using ClinicService.Domain.Entities;
using ClinicService.Domain.Repositories;

namespace ClinicService.Application.Services.Commands.CreateService;

internal sealed class CreateServiceCommandHandler(
    IServiceRepository serviceRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateServiceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateServiceCommand request, CancellationToken ct)
    {
        var service = Service.Create(request.Name, request.Category, request.DurationMinutes, request.Price, request.Description);
        await serviceRepository.AddAsync(service, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return service.Id;
    }
}
