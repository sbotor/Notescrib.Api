FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Notescrib.ApiGateway/Notescrib.ApiGateway.csproj" "Notescrib.ApiGateway/"

RUN dotnet restore "Notescrib.ApiGateway/Notescrib.ApiGateway.csproj"
COPY . .

WORKDIR "/src/Notescrib.ApiGateway"
RUN dotnet build "Notescrib.ApiGateway.csproj" -c Release -o app/build

FROM build AS publish
RUN dotnet publish "Notescrib.ApiGateway.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Notescrib.ApiGateway.dll" ]
