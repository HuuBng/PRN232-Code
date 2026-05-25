FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY PRN232.LMS.Repositories/PRN232.LMS.Repositories.csproj PRN232.LMS.Repositories/
COPY PRN232.LMS.Services/PRN232.LMS.Services.csproj PRN232.LMS.Services/
COPY PRN232.LMS.API/PRN232.LMS.API.csproj PRN232.LMS.API/
RUN dotnet restore PRN232.LMS.API/PRN232.LMS.API.csproj

COPY . .
RUN dotnet publish PRN232.LMS.API/PRN232.LMS.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "PRN232.LMS.API.dll"]
