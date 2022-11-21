FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Core/Notescrib.Core/Notescrib.Core.csproj" "Core/Notescrib.Core/"
COPY "Core/Notescrib.Core.Api/Notescrib.Core.Api.csproj" "Core/Notescrib.Core.Api/"

COPY "Services/Identity/Notescrib.Identity/Notescrib.Identity.csproj" "Services/Identity/Notescrib.Identity/"
COPY "Services/Identity/Notescrib.Identity.Api/Notescrib.Identity.Api.csproj" "Services/Identity/Notescrib.Identity.Api/"

RUN dotnet restore "Services/Identity/Notescrib.Identity.Api/Notescrib.Identity.Api.csproj"
COPY . .

WORKDIR "/src/Services/Identity/Notescrib.Identity.Api"
RUN dotnet build "Notescrib.Identity.Api.csproj" -c Release -o app/build

FROM build AS publish
RUN dotnet publish "Notescrib.Identity.Api.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Notescrib.Identity.Api.dll" ]
