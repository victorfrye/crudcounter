FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
LABEL com.github.owner="victorfrye"
LABEL com.github.repo="crudcounter"
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source

COPY src/WebApi/WebApi.csproj src/WebApi/
COPY src/ServiceDefaults/ServiceDefaults.csproj src/ServiceDefaults/

RUN dotnet restore src/WebApi/WebApi.csproj

COPY . .
RUN dotnet build src/WebApi/WebApi.csproj -c Release --no-restore

FROM build AS publish
RUN dotnet publish src/WebApi/WebApi.csproj -c Release --no-build -o /out

FROM base AS final
WORKDIR /app
COPY --from=publish /out .
ENTRYPOINT ["dotnet", "VictorFrye.CrudCounter.WebApi.dll"]
