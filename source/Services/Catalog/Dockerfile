FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY [OPTIONS] <src> ... <dest>
COPY ["source/Services/Catalog/EShopOnContainers.Catalog.csproj", "/src/Services/Catalog/"]
# INSTRUCTION ["executable","param1","param2"]
RUN ["dotnet", "restore", "/src/Services/Catalog/EShopOnContainers.Catalog.csproj"]

COPY ["source/Services/Catalog/", "/src/Services/Catalog/"]
WORKDIR /src/Services/Catalog/
RUN ["dotnet", "publish", "--configuration", "Release", "--output", "/app"]

# Each FROM instruction clears any state created by previous instructions.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build ["/app", "."]
ENTRYPOINT ["dotnet", "EShopOnContainers.Catalog.dll"]
