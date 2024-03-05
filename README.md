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

## migration commands
```bash
dotnet ef migrations add MIGRATION_NAME --project="../DigitalMenu_30_DAL"
```
```bash
dotnet ef database update --project="../DigitalMenu_30_DAL"
```
