FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Core/Notescrib.Core/Notescrib.Core.csproj" "Core/Notescrib.Core/"
COPY "Core/Notescrib.Core.Api/Notescrib.Core.Api.csproj" "Core/Notescrib.Core.Api/"

COPY "Services/Notes/Notescrib.Notes/Notescrib.Notes.csproj" "Services/Notes/Notescrib.Notes/"
COPY "Services/Notes/Notescrib.Notes.Api/Notescrib.Notes.Api.csproj" "Services/Notes/Notescrib.Notes.Api/"

RUN dotnet restore "Services/Notes/Notescrib.Notes.Api/Notescrib.Notes.Api.csproj"
COPY . .

WORKDIR "/src/Services/Notes/Notescrib.Notes.Api"
RUN dotnet build "Notescrib.Notes.Api.csproj" -c Release -o app/build

FROM build AS publish
RUN dotnet publish "Notescrib.Notes.Api.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Notescrib.Notes.Api.dll" ]
