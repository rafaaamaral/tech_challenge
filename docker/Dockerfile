FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/tech_challenge.API/tech_challenge.API.csproj", "src/tech_challenge.API/"]
COPY ["src/tech_challenge.Application/tech_challenge.Application.csproj", "src/tech_challenge.Application/"]
COPY ["src/tech_challenge.Domain/tech_challenge.Domain.csproj", "src/tech_challenge.Domain/"]
COPY ["src/tech_challenge.Infrastructure/tech_challenge.Infrastructure.csproj", "src/tech_challenge.Infrastructure/"]
COPY ["src/tech_challenge.Teste/tech_challenge.Teste.csproj", "src/tech_challenge.Teste/"]
COPY ["tech_challenge_fase_1.slnx", "./"]

RUN dotnet restore "src/tech_challenge.API/tech_challenge.API.csproj"

COPY . .

RUN dotnet publish "src/tech_challenge.API/tech_challenge.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "tech_challenge.API.dll"]