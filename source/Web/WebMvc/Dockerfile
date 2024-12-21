FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY [OPTIONS] <src> ... <dest>
COPY ["source/Web/WebMvc/EShopOnContainers.WebMvc.csproj", "/src/Web/WebMvc/"]
# INSTRUCTION ["executable","param1","param2"]
RUN ["dotnet", "restore", "/src/Web/WebMvc/EShopOnContainers.WebMvc.csproj"]

COPY ["source/Web/WebMvc/", "/src/Web/WebMvc/"]
WORKDIR /src/Web/WebMvc/
RUN ["dotnet", "publish", "--configuration", "Release", "--output", "/app"]

# Each FROM instruction clears any state created by previous instructions.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build ["/app", "."]
ENTRYPOINT ["dotnet", "EShopOnContainers.WebMvc.dll"]
