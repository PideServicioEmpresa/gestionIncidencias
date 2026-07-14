# ── Etapa 1: compilación ─────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PideServicio.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["global.json", "."]
COPY ["src/PideServicio.Api/PideServicio.Api.csproj",                 "src/PideServicio.Api/"]
COPY ["src/PideServicio.Application/PideServicio.Application.csproj", "src/PideServicio.Application/"]
COPY ["src/PideServicio.Domain/PideServicio.Domain.csproj",           "src/PideServicio.Domain/"]
COPY ["src/PideServicio.Infrastructure/PideServicio.Infrastructure.csproj", "src/PideServicio.Infrastructure/"]
COPY ["src/PideServicio.Persistence/PideServicio.Persistence.csproj", "src/PideServicio.Persistence/"]
COPY ["src/PideServicio.Contracts/PideServicio.Contracts.csproj",     "src/PideServicio.Contracts/"]

RUN dotnet restore "src/PideServicio.Api/PideServicio.Api.csproj"

COPY . .

RUN dotnet publish "src/PideServicio.Api/PideServicio.Api.csproj" \
    -c Release \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

# ── Etapa 2: imagen de runtime ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/logs

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PideServicio.Api.dll"]
