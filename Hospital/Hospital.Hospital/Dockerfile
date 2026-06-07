FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Hospital.Hospital/Hospital.Hospital.csproj", "Hospital.Hospital/"]
COPY ["Hospital.Core/Hospital.Core.csproj", "Hospital.Core/"]
COPY ["Hospital.Db/Hospital.Db.csproj", "Hospital.Db/"]
COPY ["Hospital.Services/Hospital.Services.csproj", "Hospital.Services/"]

RUN dotnet restore "Hospital.Hospital/Hospital.Hospital.csproj"

COPY . .

WORKDIR "/src/Hospital.Hospital"
RUN dotnet publish "Hospital.Hospital.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Hospital.Hospital.dll"]