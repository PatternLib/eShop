FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY [OPTIONS] <src> ... <dest>
COPY ["source/Services/Basket/EShopOnContainers.Basket.csproj", "/src/Services/Basket/"]
# INSTRUCTION ["executable","param1","param2"]
RUN ["dotnet", "restore", "/src/Services/Basket/EShopOnContainers.Basket.csproj"]

COPY ["source/Services/Basket/", "/src/Services/Basket/"]
WORKDIR /src/Services/Basket/
RUN ["dotnet", "publish", "--configuration", "Release", "--output", "/app"]

# Each FROM instruction clears any state created by previous instructions.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build ["/app", "."]
ENTRYPOINT ["dotnet", "EShopOnContainers.Basket.dll"]
