FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PRN232.LMS.API/PRN232.LMS.API.csproj", "PRN232.LMS.API/"]
RUN dotnet restore "PRN232.LMS.API/PRN232.LMS.API.csproj"
COPY . .
WORKDIR "/src/PRN232.LMS.API"
RUN dotnet build "./PRN232.LMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PRN232.LMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]
