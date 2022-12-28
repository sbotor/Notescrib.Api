FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Core/Notescrib.Core/Notescrib.Core.csproj" "Core/Notescrib.Core/"
COPY "Core/Notescrib.Core.Api/Notescrib.Core.Api.csproj" "Core/Notescrib.Core.Api/"

COPY "Services/Emails/Notescrib.Emails/Notescrib.Emails.csproj" "Services/Emails/Notescrib.Emails/"
COPY "Services/Emails/Notescrib.Emails.Api/Notescrib.Emails.Api.csproj" "Services/Emails/Notescrib.Emails.Api/"

RUN dotnet restore "Services/Emails/Notescrib.Emails.Api/Notescrib.Emails.Api.csproj"
COPY . .

WORKDIR "/src/Services/Emails/Notescrib.Emails.Api"
RUN dotnet build "Notescrib.Emails.Api.csproj" -c Release -o app/build

FROM build AS publish
RUN dotnet publish "Notescrib.Emails.Api.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Notescrib.Emails.Api.dll" ]
