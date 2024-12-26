FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY [OPTIONS] <src> ... <dest>
COPY ["source/Services/Identity/EShopOnContainers.Identity.csproj", "/src/Services/Identity/"]
# INSTRUCTION ["executable","param1","param2"]
RUN ["dotnet", "restore", "/src/Services/Identity/EShopOnContainers.Identity.csproj"]

COPY ["source/Services/Identity/", "/src/Services/Identity/"]
WORKDIR /src/Services/Identity/
RUN ["dotnet", "publish", "--configuration", "Release", "--output", "/app"]

# Each FROM instruction clears any state created by previous instructions.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build ["/app", "."]
ENTRYPOINT ["dotnet", "EShopOnContainers.Identity.dll"]
