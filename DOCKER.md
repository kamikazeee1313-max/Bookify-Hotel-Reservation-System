# 🐳 Bookify - راهنمای Docker

<div dir="rtl">

این راهنما نحوه استفاده از Docker برای راه‌اندازی سریع و آسان پروژه Bookify را توضیح می‌دهد.

## 📑 فهرست مطالب

1. [مزایای استفاده از Docker](#-مزایای-استفاده-از-docker)
2. [پیشنیازها](#-پیشنیازها)
3. [راه‌اندازی سریع](#-راه‌اندازی-سریع)
4. [راه‌اندازی جزئی](#-راه‌اندازی-جزئی)
5. [مدیریت Containers](#-مدیریت-containers)
6. [رفع مشکلات](#-رفع-مشکلات)

---

## 🎯 مزایای استفاده از Docker

✅ **راه‌اندازی سریع**: بدون نیاز به نصب SQL Server یا تنظیمات پیچیده  
✅ **سازگاری**: اجرای یکسان در تمام سیستم‌عامل‌ها  
✅ **ایزوله**: بدون تداخل با برنامه‌های دیگر  
✅ **قابل حمل**: به راحتی قابل انتقال به سرورهای مختلف  
✅ **مقیاس‌پذیری**: امکان Scale کردن آسان  

---

## 📋 پیشنیازها

### 1. نصب Docker Desktop

#### ویندوز
1. از [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop) دانلود کنید
2. نصب کرده و کامپیوتر را Restart کنید
3. Docker Desktop را اجرا کنید
4. WSL 2 را فعال کنید (در صورت درخواست)

#### macOS
```bash
# با Homebrew
brew install --cask docker

# یا دانلود مستقیم از:
# https://www.docker.com/products/docker-desktop
```

#### Linux (Ubuntu/Debian)
```bash
# حذف نسخه‌های قدیمی
sudo apt-get remove docker docker-engine docker.io containerd runc

# نصب پیشنیازها
sudo apt-get update
sudo apt-get install ca-certificates curl gnupg lsb-release

# افزودن Docker repository
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# نصب Docker
sudo apt-get update
sudo apt-get install docker-ce docker-ce-cli containerd.io docker-compose-plugin

# اجرای Docker بدون sudo
sudo usermod -aG docker $USER
newgrp docker
```

### 2. بررسی نصب Docker

```bash
# بررسی نسخه Docker
docker --version

# بررسی Docker Compose
docker compose version

# تست Docker
docker run hello-world
```

---

## 🚀 راه‌اندازی سریع

### گزینه 1: استفاده از docker-compose (توصیه می‌شود)

ایجاد فایل `docker-compose.yml` در root پروژه:

```yaml
version: '3.8'

services:
  # SQL Server Database
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: bookify-sqlserver
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Password123
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - bookify-network
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 3s
      retries: 10
      start_period: 10s

  # Bookify API Application
  bookify-api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: bookify-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=BookifyDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;
      - Jwt__Key=YourSuperSecretKeyForJWTAuthenticationMustBeAtLeast32CharactersLongPleaseChangeThis!
      - Jwt__Issuer=BookifyApi
      - Jwt__Audience=BookifyClient
      - Jwt__DurationInMinutes=60
    ports:
      - "5080:80"
      - "7280:443"
    depends_on:
      sqlserver:
        condition: service_healthy
    networks:
      - bookify-network
    restart: unless-stopped

volumes:
  sqlserver_data:
    driver: local

networks:
  bookify-network:
    driver: bridge
```

### ایجاد Dockerfile

ایجاد فایل `Dockerfile` در root پروژه:

```dockerfile
# استفاده از SDK برای Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# کپی فایل‌های csproj و restore
COPY ["Bookify.Api/Bookify.Api.csproj", "Bookify.Api/"]
COPY ["Bookify.Application/Bookify.Application.Business.csproj", "Bookify.Application/"]
COPY ["Bookify.Domain/Bookify.Domain.csproj", "Bookify.Domain/"]
COPY ["Bookify.Infrastructure/Bookify.Infrastructure.Data.csproj", "Bookify.Infrastructure/"]
COPY ["Bookify.Shared/Bookify.Shared.csproj", "Bookify.Shared/"]

RUN dotnet restore "Bookify.Api/Bookify.Api.csproj"

# کپی بقیه فایل‌ها و build
COPY . .
WORKDIR "/src/Bookify.Api"
RUN dotnet build "Bookify.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Bookify.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# استفاده از Runtime برای اجرا
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# نصب dotnet-ef برای Migration
RUN dotnet tool install --global dotnet-ef --version 9.*
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY --from=publish /app/publish .

# اجرای Migration و سپس برنامه
COPY ["Bookify.Api/entrypoint.sh", "/app/"]
RUN chmod +x /app/entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh"]
```

### ایجاد entrypoint.sh

ایجاد فایل `Bookify.Api/entrypoint.sh`:

```bash
#!/bin/bash
set -e

echo "Waiting for SQL Server to be ready..."
sleep 20

echo "Running database migrations..."
dotnet ef database update --project /src/Bookify.Infrastructure/Bookify.Infrastructure.Data.csproj --startup-project /src/Bookify.Api/Bookify.Api.csproj || echo "Migration failed or already applied"

echo "Starting application..."
dotnet Bookify.Api.dll
```

### اجرا

```bash
# Build و Start کردن تمام سرویس‌ها
docker compose up -d

# مشاهده logs
docker compose logs -f

# برای توقف
docker compose down

# برای توقف و حذف volumes (داده‌ها پاک می‌شوند)
docker compose down -v
```

---

## 🔧 راه‌اندازی جزئی

### فقط SQL Server در Docker

اگر فقط می‌خواهید SQL Server را در Docker اجرا کنید و برنامه را local اجرا کنید:

```bash
# اجرای SQL Server
docker run -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=YourStrong@Password123" \
  -p 1433:1433 \
  --name bookify-sqlserver \
  --hostname bookify-sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest

# بررسی وضعیت
docker ps

# مشاهده logs
docker logs bookify-sqlserver

# توقف
docker stop bookify-sqlserver

# شروع مجدد
docker start bookify-sqlserver

# حذف
docker rm bookify-sqlserver
```

سپس Connection String را در `appsettings.json` تنظیم کنید:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BookifyDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
  }
}
```

### Build و اجرای فقط API

```bash
# Build image
docker build -t bookify-api:latest .

# اجرا (فرض بر اینکه SQL Server در حال اجراست)
docker run -d \
  --name bookify-api \
  -p 5080:80 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=BookifyDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;" \
  -e Jwt__Key="YourSuperSecretKeyForJWTAuthenticationMustBeAtLeast32CharactersLongPleaseChangeThis!" \
  -e Jwt__Issuer="BookifyApi" \
  -e Jwt__Audience="BookifyClient" \
  bookify-api:latest

# مشاهده logs
docker logs -f bookify-api
```

> **نکته**: `host.docker.internal` برای اتصال به SQL Server که روی Host اجرا می‌شود استفاده می‌شود.

---

## 🛠️ مدیریت Containers

### دستورات پایه Docker

```bash
# لیست containerهای در حال اجرا
docker ps

# لیست تمام containerها (شامل متوقف شده)
docker ps -a

# لیست imageها
docker images

# مشاهده logs
docker logs <container-name>
docker logs -f <container-name>  # real-time

# ورود به container
docker exec -it <container-name> bash

# توقف container
docker stop <container-name>

# شروع container
docker start <container-name>

# Restart container
docker restart <container-name>

# حذف container
docker rm <container-name>
docker rm -f <container-name>  # force remove

# حذف image
docker rmi <image-name>

# پاک کردن تمام containerها و imageهای استفاده نشده
docker system prune -a
```

### دستورات Docker Compose

```bash
# Start (ساخت و اجرا)
docker compose up

# Start در background
docker compose up -d

# Build مجدد
docker compose up --build

# توقف سرویس‌ها
docker compose stop

# شروع مجدد
docker compose start

# Restart
docker compose restart

# توقف و حذف
docker compose down

# مشاهده logs
docker compose logs
docker compose logs -f  # real-time
docker compose logs <service-name>

# لیست سرویس‌ها
docker compose ps

# اجرای دستور در container
docker compose exec <service-name> bash

# Scale کردن سرویس
docker compose up -d --scale bookify-api=3
```

### اتصال به SQL Server داخل Container

```bash
# ورود به container
docker exec -it bookify-sqlserver bash

# اجرای sqlcmd
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123"

# یا مستقیم:
docker exec -it bookify-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123"
```

در sqlcmd:
```sql
-- لیست دیتابیس‌ها
SELECT name FROM sys.databases;
GO

-- استفاده از دیتابیس
USE BookifyDB;
GO

-- لیست جداول
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;
GO

-- خروج
EXIT
```

---

## 🐛 رفع مشکلات

### مشکل: Container بلافاصله متوقف می‌شود

```bash
# مشاهده logs برای پیدا کردن خطا
docker logs bookify-api
docker logs bookify-sqlserver
```

**راه حل‌های رایج:**
- بررسی متغیرهای محیطی
- بررسی Connection String
- اطمینان از اینکه SQL Server آماده است

### مشکل: نمی‌توانم به SQL Server متصل شوم

**راه حل:**
```bash
# بررسی وضعیت health
docker inspect bookify-sqlserver | grep -A 20 Health

# تست اتصال
docker exec bookify-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "SELECT 1"
```

### مشکل: Port قبلاً در حال استفاده است

```bash
# پیدا کردن process که از port استفاده می‌کند

# ویندوز
netstat -ano | findstr :1433
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :1433
kill -9 <PID>

# یا تغییر port در docker-compose.yml
ports:
  - "1434:1433"  # پورت خارجی را تغییر دهید
```

### مشکل: Migration اجرا نمی‌شود

**راه حل دستی:**

```bash
# ورود به container
docker exec -it bookify-api bash

# اجرای migration
cd /src
dotnet ef database update --project Bookify.Infrastructure/Bookify.Infrastructure.Data.csproj --startup-project Bookify.Api/Bookify.Api.csproj
```

### مشکل: حافظه کافی نیست

Docker Desktop محدودیت حافظه دارد:

1. Docker Desktop را باز کنید
2. Settings > Resources
3. Memory را افزایش دهید (حداقل 4GB)
4. Apply & Restart

### مشکل: Build خیلی کند است

**بهینه‌سازی Dockerfile:**

```dockerfile
# استفاده از .dockerignore
# ایجاد فایل .dockerignore در root:

bin/
obj/
.vs/
.vscode/
*.user
*.suo
.git/
node_modules/
Logs/
```

### مشکل: تغییرات کد اعمال نمی‌شوند

```bash
# Build مجدد بدون cache
docker compose build --no-cache

# یا
docker compose up --build --force-recreate
```

---

## 📊 Monitoring و Logs

### مشاهده استفاده از منابع

```bash
# استفاده از CPU و Memory
docker stats

# برای containerهای خاص
docker stats bookify-api bookify-sqlserver
```

### Export و Import Data

```bash
# Backup دیتابیس
docker exec bookify-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "BACKUP DATABASE BookifyDB TO DISK='/var/opt/mssql/backup/BookifyDB.bak'"

# کپی backup به host
docker cp bookify-sqlserver:/var/opt/mssql/backup/BookifyDB.bak ./BookifyDB.bak

# Restore
docker cp ./BookifyDB.bak bookify-sqlserver:/var/opt/mssql/backup/
docker exec bookify-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "RESTORE DATABASE BookifyDB FROM DISK='/var/opt/mssql/backup/BookifyDB.bak' WITH REPLACE"
```

---

## 🌐 Deploy به Production

### استفاده از Docker Hub

```bash
# Login
docker login

# Tag کردن image
docker tag bookify-api:latest yourusername/bookify-api:v1.0.0
docker tag bookify-api:latest yourusername/bookify-api:latest

# Push
docker push yourusername/bookify-api:v1.0.0
docker push yourusername/bookify-api:latest
```

### استفاده از Private Registry

```bash
# Tag برای private registry
docker tag bookify-api:latest registry.yourcompany.com/bookify-api:v1.0.0

# Push
docker push registry.yourcompany.com/bookify-api:v1.0.0
```

### Docker Compose برای Production

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${DB_PASSWORD}
      - MSSQL_PID=Standard
    volumes:
      - sqlserver_data:/var/opt/mssql
    restart: always
    
  bookify-api:
    image: yourusername/bookify-api:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${CONNECTION_STRING}
      - Jwt__Key=${JWT_SECRET}
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - sqlserver
    restart: always

volumes:
  sqlserver_data:
```

---

## 🎓 نکات و بهترین روش‌ها

1. **از .env file استفاده کنید** برای مدیریت متغیرهای محیطی
2. **Health Checks تعریف کنید** برای اطمینان از آمادگی سرویس‌ها
3. **Volumes استفاده کنید** برای persist کردن data
4. **Log aggregation** برای Production
5. **Secrets مدیریت شوند** با Docker Secrets یا HashiCorp Vault
6. **Multi-stage builds** برای کاهش سایز image
7. **از image های official استفاده کنید**
8. **Regular updates** برای Security patches

---

<div align="center">

**راه‌اندازی موفقیت‌آمیز با Docker! 🎉**

[⬆ بازگشت به بالا](#-bookify---راهنمای-docker)

**ساخته شده با ❤️ توسط تیم pentaRae**

</div>

</div>
