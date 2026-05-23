FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]
COPY ["src/Shared/Shared.BuildingBlocks/Shared.BuildingBlocks.csproj", "src/Shared/Shared.BuildingBlocks/"]
COPY ["src/ClinicService/ClinicService.Domain/ClinicService.Domain.csproj", "src/ClinicService/ClinicService.Domain/"]
COPY ["src/ClinicService/ClinicService.Application/ClinicService.Application.csproj", "src/ClinicService/ClinicService.Application/"]
COPY ["src/ClinicService/ClinicService.Infrastructure/ClinicService.Infrastructure.csproj", "src/ClinicService/ClinicService.Infrastructure/"]
COPY ["src/ClinicService/ClinicService.API/ClinicService.API.csproj", "src/ClinicService/ClinicService.API/"]

RUN dotnet restore "src/ClinicService/ClinicService.API/ClinicService.API.csproj"

COPY . .
RUN dotnet build "src/ClinicService/ClinicService.API/ClinicService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/ClinicService/ClinicService.API/ClinicService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClinicService.API.dll"]
