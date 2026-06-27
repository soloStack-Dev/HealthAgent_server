FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY HealthAgent.Api.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8080
ENV DatabaseProvider=Sqlite
ENV ConnectionStrings__SqliteConnection="Data Source=HealthAgent.db"
ENTRYPOINT ["dotnet", "HealthAgent.Api.dll"]
