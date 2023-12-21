FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Notescrib.Core/Notescrib.Core.csproj" "Notescrib.Core/"
COPY "Notescrib.Core.Api/Notescrib.Core.Api.csproj" "Notescrib.Core.Api/"

COPY "Notescrib/Notescrib.csproj" "Notescrib/"
COPY "Notescrib.WebApi/Notescrib.WebApi.csproj" "Notescrib.WebApi/"

RUN dotnet restore "Notescrib.WebApi/Notescrib.WebApi.csproj"
COPY . .

WORKDIR "/src/Notescrib.WebApi"
RUN dotnet build "Notescrib.WebApi.csproj" -c Release -o app/build

FROM build AS publish
RUN dotnet publish "Notescrib.WebApi.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Notescrib.WebApi.dll" ]
