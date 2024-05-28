# DigitalMenu

## Getting started
Create docker network named "digitalmenu_network"
```bash
docker network create digitalmenu_network
```
Add this network to the docker start script  
And add the following environment variable(s) to the docker start script  
```env
ConnectionStrings__DefaultConnection="Server=digitalmenu-mysql;Port=3306;Database=digitalmenu;User ID=user;Password=password;"
```  
Run the following command in the root folder (where the .sln is located) to start the docker containers
```bash
docker-compose up -d
```
Exec into the app container to run dotnet (ef) commands  
```bash
docker-compose exec app bash
```

To start the application container you can start it via Visual Studio

For the translations to work, you need to run the docker command because it is too slow for docker-compose
```bash
docker run -it --name translator -p 8888:5000 libretranslate/libretranslate
```

## migration commands
Add/change the connection string in the appsettings.json to:
```text
Server=localhost;Port=3306;Database=digitalmenu;User ID=user;Password=password;
```
Run the following commands in the project folder DigitalMenu_10_Api
```bash
dotnet ef migrations add MIGRATION_NAME --project="../DigitalMenu_30_DAL"
```
```bash
dotnet ef database update --project="../DigitalMenu_30_DAL"
```

## ReSharper
Please run this command before pushing to the repository
```bash
jb cleanupcode --no-build .\DigitalMenu.sln -o="resharper_log.yml"
```
