FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/PRN232.LMS.Shared/PRN232.LMS.Shared.csproj src/PRN232.LMS.Shared/
COPY src/PRN232.LMS.Protos/PRN232.LMS.Protos.csproj src/PRN232.LMS.Protos/
COPY src/Services/PRN232.LMS.StudentService/PRN232.LMS.StudentService.csproj src/Services/PRN232.LMS.StudentService/
RUN dotnet restore src/Services/PRN232.LMS.StudentService/PRN232.LMS.StudentService.csproj

COPY . .
RUN dotnet publish src/Services/PRN232.LMS.StudentService/PRN232.LMS.StudentService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
EXPOSE 5001
ENTRYPOINT ["dotnet", "PRN232.LMS.StudentService.dll"]
