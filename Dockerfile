# Use the official .NET 9 runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9 SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the Directory.Build.props file first
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy the project files
COPY ["src/Litrater.Presentation/Litrater.Presentation.csproj", "src/Litrater.Presentation/"]
COPY ["src/Litrater.Application/Litrater.Application.csproj", "src/Litrater.Application/"]
COPY ["src/Litrater.Infrastructure/Litrater.Infrastructure.csproj", "src/Litrater.Infrastructure/"]
COPY ["src/Litrater.Domain/Litrater.Domain.csproj", "src/Litrater.Domain/"]

# Restore dependencies
RUN dotnet restore "src/Litrater.Presentation/Litrater.Presentation.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/src/Litrater.Presentation"
RUN dotnet build "Litrater.Presentation.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Litrater.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Litrater.Presentation.dll"] 