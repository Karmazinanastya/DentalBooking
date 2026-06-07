FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/AdminWebApp/AdminWebApp.csproj", "src/AdminWebApp/"]
RUN dotnet restore "src/AdminWebApp/AdminWebApp.csproj"

COPY . .
RUN dotnet build "src/AdminWebApp/AdminWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/AdminWebApp/AdminWebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminWebApp.dll"]
