FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/PRN232.LMS.Shared/PRN232.LMS.Shared.csproj src/PRN232.LMS.Shared/
COPY src/PRN232.LMS.Protos/PRN232.LMS.Protos.csproj src/PRN232.LMS.Protos/
COPY src/Services/PRN232.LMS.CourseService/PRN232.LMS.CourseService.csproj src/Services/PRN232.LMS.CourseService/
RUN dotnet restore src/Services/PRN232.LMS.CourseService/PRN232.LMS.CourseService.csproj

COPY . .
RUN dotnet publish src/Services/PRN232.LMS.CourseService/PRN232.LMS.CourseService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PRN232.LMS.CourseService.dll"]
