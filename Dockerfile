# Use ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy only the WebFrontend project (not the whole solution tree)
WORKDIR /src
COPY ["SalesTrack.WebFrontend/SalesTrack.WebFrontend.csproj", "SalesTrack.WebFrontend/"]
RUN dotnet restore "SalesTrack.WebFrontend/SalesTrack.WebFrontend.csproj"

# Copy the rest of the project and build it
COPY . .
WORKDIR "/src/SalesTrack.WebFrontend"
RUN dotnet publish "SalesTrack.WebFrontend.csproj" -c Release -o /app/publish

# Copy source code and restore dependencies
COPY . .
RUN dotnet restore SalesTrack.sln

# Build and publish the main project
RUN dotnet publish SalesTrack.WebFrontend/SalesTrack.WebFrontend.csproj -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set to Production
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SalesTrack.WebFrontend.dll"]
