FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]
COPY ["src/Shared/Shared.BuildingBlocks/Shared.BuildingBlocks.csproj", "src/Shared/Shared.BuildingBlocks/"]
COPY ["src/PatientService/PatientService.Domain/PatientService.Domain.csproj", "src/PatientService/PatientService.Domain/"]
COPY ["src/PatientService/PatientService.Application/PatientService.Application.csproj", "src/PatientService/PatientService.Application/"]
COPY ["src/PatientService/PatientService.Infrastructure/PatientService.Infrastructure.csproj", "src/PatientService/PatientService.Infrastructure/"]
COPY ["src/PatientService/PatientService.API/PatientService.API.csproj", "src/PatientService/PatientService.API/"]

RUN dotnet restore "src/PatientService/PatientService.API/PatientService.API.csproj"

COPY . .
RUN dotnet build "src/PatientService/PatientService.API/PatientService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/PatientService/PatientService.API/PatientService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientService.API.dll"]
