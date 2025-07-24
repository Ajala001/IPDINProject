# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY IPDINProject.API/IPDINProject.API.csproj IPDINProject.API/
RUN dotnet restore IPDINProject.API/IPDINProject.API.csproj

COPY . .
WORKDIR /src/IPDINProject.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "IPDINProject.API.dll"]
