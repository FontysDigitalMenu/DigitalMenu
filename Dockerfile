FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY . .

WORKDIR /app/DigitalMenu_10_Api

ENTRYPOINT ["tail"]
CMD ["-f","/dev/null"]