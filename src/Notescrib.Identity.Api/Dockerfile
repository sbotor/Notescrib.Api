FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Notescrib.Core/Notescrib.Core.csproj" "Notescrib.Core/"
COPY "Notescrib.Core.Api/Notescrib.Core.Api.csproj" "Notescrib.Core.Api/"

COPY "Notescrib.Identity/Notescrib.Identity.csproj" "Notescrib.Identity/"
COPY "Notescrib.Identity.Api/Notescrib.Identity.Api.csproj" "Notescrib.Identity.Api/"

RUN dotnet restore "Notescrib.Identity.Api/Notescrib.Identity.Api.csproj"
COPY . .

WORKDIR "/src/Notescrib.Identity.Api"
RUN dotnet build "Notescrib.Identity.Api.csproj" -c Release -o app/build

FROM build AS publish
RUN dotnet publish "Notescrib.Identity.Api.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Notescrib.Identity.Api.dll" ]
