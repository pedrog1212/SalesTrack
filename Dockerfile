# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy solution and project files first (for layer caching)
COPY *.sln ./
COPY SalesTrack.WebFrontend/*.csproj SalesTrack.WebFrontend/
COPY SalesTrack.CRM/*.csproj SalesTrack.CRM/
COPY SalesTrack.Shared.DTOs/*.csproj SalesTrack.Shared.DTOs/
COPY SalesTrack.KPIService/*.csproj SalesTrack.KPIService/

# Restore
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build and publish
WORKDIR /src/SalesTrack.WebFrontend
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SalesTrack.WebFrontend.dll"]
