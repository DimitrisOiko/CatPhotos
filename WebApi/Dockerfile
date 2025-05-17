FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80

COPY CatPhotos.sln CatPhotos.sln
COPY WebApi/WebApi.csproj WebApi/WebApi.csproj
COPY WebApi.Tests/WebApi.Tests.csproj WebApi.Tests/WebApi.Tests.csproj

RUN dotnet restore CatPhotos.sln

# Copy all files and build
COPY WebApi WebApi
WORKDIR /app/WebApi
RUN dotnet publish -c Release -o /out

# Use the runtime image for final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "WebApi.dll"]