FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Hospital.Auth/Hospital.Auth.csproj", "Hospital.Auth/"]
COPY ["Hospital.Db/Hospital.Db.csproj", "Hospital.Db/"]
COPY ["Hospital.Services/Hospital.Services.csproj", "Hospital.Services/"]

RUN dotnet restore "Hospital.Auth/Hospital.Auth.csproj"

COPY . .

WORKDIR "/src/Hospital.Auth"
RUN dotnet publish "Hospital.Auth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Hospital.Auth.dll"]