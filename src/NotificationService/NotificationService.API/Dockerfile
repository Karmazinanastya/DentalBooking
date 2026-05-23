FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]
COPY ["src/NotificationService/NotificationService.Application/NotificationService.Application.csproj", "src/NotificationService/NotificationService.Application/"]
COPY ["src/NotificationService/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "src/NotificationService/NotificationService.Infrastructure/"]
COPY ["src/NotificationService/NotificationService.API/NotificationService.API.csproj", "src/NotificationService/NotificationService.API/"]

RUN dotnet restore "src/NotificationService/NotificationService.API/NotificationService.API.csproj"

COPY . .
RUN dotnet build "src/NotificationService/NotificationService.API/NotificationService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/NotificationService/NotificationService.API/NotificationService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
