#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install tzdata and set the timezone to Amsterdam
USER root
RUN apt-get update && apt-get install -y tzdata \
    && ln -snf /usr/share/zoneinfo/Europe/Amsterdam /etc/localtime \
    && echo "Europe/Amsterdam" > /etc/timezone

# Ensure the app user has permission to the necessary directories
RUN chown -R app:app /app /home/app

# Switch back to non-root user
USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DigitalMenu_10_Api/DigitalMenu_10_Api.csproj", "DigitalMenu_10_Api/"]
COPY ["DigitalMenu_30_DAL/DigitalMenu_30_DAL.csproj", "DigitalMenu_30_DAL/"]
COPY ["DigitalMenu_20_BLL/DigitalMenu_20_BLL.csproj", "DigitalMenu_20_BLL/"]
RUN dotnet restore "./DigitalMenu_10_Api/DigitalMenu_10_Api.csproj"
COPY . .
WORKDIR "/src/DigitalMenu_10_Api"
RUN dotnet build "./DigitalMenu_10_Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DigitalMenu_10_Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalMenu_10_Api.dll"]
