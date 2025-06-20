FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5241

ENV ASPNETCORE_URLS=http://+:5241

# Cria a pasta de uploads com permissão de escrita
RUN mkdir /app/Storage && chmod -R 777 /app/Storage

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["WebApi/WebApi.csproj", "WebApi/"]
RUN dotnet restore "WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/WebApi"
RUN dotnet build "WebApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "WebApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]