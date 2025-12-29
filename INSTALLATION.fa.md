# 📦 راهنمای نصب کامل Bookify

<div dir="rtl">

این راهنما شامل دستورالعمل‌های گام‌به‌گام برای نصب و راه‌اندازی سیستم رزرو هتل Bookify است.

## 📑 فهرست مطالب

1. [نصب پیشنیازها](#1-نصب-پیشنیازها)
2. [پیکربندی دیتابیس](#2-پیکربندی-دیتابیس)
3. [دانلود و راه‌اندازی پروژه](#3-دانلود-و-راه‌اندازی-پروژه)
4. [تنظیمات محیطی](#4-تنظیمات-محیطی)
5. [اجرای Migration](#5-اجرای-migration)
6. [راه‌اندازی برنامه](#6-راه‌اندازی-برنامه)
7. [رفع مشکلات رایج](#7-رفع-مشکلات-رایج)

---

## 1. نصب پیشنیازها

### 1.1. نصب .NET SDK

#### ویندوز

1. به سایت [Microsoft .NET](https://dotnet.microsoft.com/download/dotnet/10.0) بروید
2. آخرین نسخه **.NET 10.0 SDK** را دانلود کنید
3. فایل نصب را اجرا کرده و مراحل را دنبال کنید
4. پس از نصب، Powershell یا CMD را باز کنید و نسخه را بررسی کنید:

```powershell
dotnet --version
```

خروجی باید شبیه به `10.0.101` باشد.

#### macOS

```bash
# با استفاده از Homebrew
brew install --cask dotnet-sdk

# بررسی نسخه
dotnet --version
```

#### Linux (Ubuntu/Debian)

```bash
# افزودن Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# نصب .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0

# بررسی نسخه
dotnet --version
```

### 1.2. نصب SQL Server

#### ویندوز

**گزینه 1: SQL Server Developer Edition (توصیه می‌شود)**

1. از [SQL Server Downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) برنامه نصب را دانلود کنید
2. گزینه **Developer** را انتخاب کنید (رایگان است)
3. نوع نصب **Basic** را انتخاب کنید
4. محل نصب را مشخص کرده و روی **Install** کلیک کنید
5. پس از نصب، اطلاعات اتصال را یادداشت کنید

**گزینه 2: SQL Server Express (سبک‌تر)**

1. [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) را دانلود کنید
2. برنامه نصب را اجرا کنید
3. گزینه **Custom** Installation را انتخاب کنید
4. در قسمت Instance Configuration:
   - Named instance: `SQLEXPRESS`
   - Instance ID: `SQLEXPRESS`
5. در قسمت Server Configuration:
   - SQL Server Database Engine را فعال کنید
6. Authentication Mode: **Mixed Mode** را انتخاب کرده و رمز عبور sa را تعیین کنید

#### macOS & Linux

SQL Server برای macOS به صورت مستقیم پشتیبانی نمی‌شود. گزینه‌های جایگزین:

**گزینه 1: استفاده از Docker (توصیه می‌شود)**

```bash
# دانلود و اجرای SQL Server در Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password123" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest

# بررسی وضعیت
docker ps
```

**گزینه 2: استفاده از SQL Server on Linux**

```bash
# افزودن Microsoft repository
curl https://packages.microsoft.com/keys/microsoft.asc | sudo tee /etc/apt/trusted.gpg.d/microsoft.asc

sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list)"

# نصب SQL Server
sudo apt-get update
sudo apt-get install -y mssql-server

# پیکربندی SQL Server
sudo /opt/mssql/bin/mssql-conf setup
```

### 1.3. نصب SQL Server Management Studio (SSMS) - اختیاری

SSMS ابزار گرافیکی برای مدیریت SQL Server است.

1. از [Download SSMS](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) دانلود کنید
2. فایل نصب را اجرا کنید
3. مراحل نصب را تکمیل کنید

**جایگزین‌های SSMS:**
- **Azure Data Studio** (کراس پلتفرم)
- **DBeaver** (رایگان و کراس پلتفرم)
- **VS Code با SQL Server Extension**

### 1.4. نصب Git

#### ویندوز
1. از [Git for Windows](https://git-scm.com/download/win) دانلود کنید
2. نصب کنید با تنظیمات پیش‌فرض

#### macOS
```bash
brew install git
```

#### Linux
```bash
sudo apt-get install git
```

بررسی نصب:
```bash
git --version
```

### 1.5. نصب Visual Studio یا VS Code

#### Visual Studio 2022 (توصیه برای ویندوز)

1. از [Visual Studio Downloads](https://visualstudio.microsoft.com/downloads/) دانلود کنید
2. حین نصب، Workload های زیر را انتخاب کنید:
   - **ASP.NET and web development**
   - **.NET desktop development**
   - **Data storage and processing**

#### Visual Studio Code (کراس پلتفرم)

1. از [VS Code](https://code.visualstudio.com/) دانلود کنید
2. افزونه‌های زیر را نصب کنید:
   - **C# for Visual Studio Code**
   - **NuGet Package Manager**
   - **SQL Server (mssql)**
   - **.NET Core Test Explorer**

---

## 2. پیکربندی دیتابیس

### 2.1. اتصال به SQL Server

#### با استفاده از SSMS

1. SQL Server Management Studio را باز کنید
2. در پنجره Connect to Server:
   - **Server name**: 
     - برای نصب محلی: `localhost` یا `(localdb)\MSSQLLocalDB`
     - برای SQL Express: `localhost\SQLEXPRESS`
   - **Authentication**: Windows Authentication یا SQL Server Authentication
   - اگر SQL Server Authentication:
     - Login: `sa`
     - Password: رمزی که در نصب تعیین کردید

#### با استفاده از Azure Data Studio

1. Azure Data Studio را باز کنید
2. روی **New Connection** کلیک کنید
3. اطلاعات مشابه بالا را وارد کنید

### 2.2. ایجاد دیتابیس (اختیاری)

دیتابیس به صورت خودکار توسط Entity Framework Core ایجاد می‌شود، اما اگر می‌خواهید دستی ایجاد کنید:

```sql
-- ایجاد دیتابیس
CREATE DATABASE BookifyDB;
GO

-- استفاده از دیتابیس
USE BookifyDB;
GO
```

### 2.3. تنظیم کاربر دیتابیس (برای محیط Production)

```sql
-- ایجاد Login
CREATE LOGIN BookifyUser WITH PASSWORD = 'YourSecurePassword123!';
GO

-- ایجاد User در دیتابیس
USE BookifyDB;
CREATE USER BookifyUser FOR LOGIN BookifyUser;
GO

-- دادن دسترسی‌ها
ALTER ROLE db_owner ADD MEMBER BookifyUser;
GO
```

---

## 3. دانلود و راه‌اندازی پروژه

### 3.1. Clone کردن پروژه

```bash
# Clone از GitHub
git clone https://github.com/kamikazeee1313-max/Bookify-Hotel-Reservation-System.git

# ورود به پوشه پروژه
cd Bookify-Hotel-Reservation-System
```

### 3.2. بررسی ساختار پروژه

```bash
# مشاهده فایل‌های Solution
ls -la

# خروجی شامل:
# - Bookify.sln (فایل Solution)
# - پوشه‌های مختلف پروژه‌ها
```

### 3.3. بازیابی پکیج‌ها

```bash
# بازیابی تمام پکیج‌های NuGet
dotnet restore

# یا باز کردن Solution در Visual Studio که خودکار restore می‌کند
```

این دستور تمام پکیج‌های مورد نیاز را از NuGet دانلود و نصب می‌کند:
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- AutoMapper
- Serilog
- Stripe.net
- و غیره...

---

## 4. تنظیمات محیطی

### 4.1. ایجاد فایل appsettings.json

در پوشه `Bookify.Api`، یک فایل `appsettings.json` ایجاد کنید (اگر وجود ندارد):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWTAuthenticationMustBeAtLeast32CharactersLong!",
    "Issuer": "BookifyApi",
    "Audience": "BookifyClient",
    "DurationInMinutes": 60
  },
  "Stripe": {
    "SecretKey": "sk_test_51234567890abcdefghijklmnopqrstuvwxyz",
    "PublishableKey": "pk_test_51234567890abcdefghijklmnopqrstuvwxyz"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/bookify-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 4.2. تنظیم Connection String

بسته به پیکربندی SQL Server خود، Connection String را تغییر دهید:

#### Windows Authentication (توصیه برای Development)
```json
"DefaultConnection": "Server=localhost;Database=BookifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### SQL Server Authentication
```json
"DefaultConnection": "Server=localhost;Database=BookifyDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
```

#### SQL Server Express
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=BookifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### LocalDB
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

#### Docker (Linux/Mac)
```json
"DefaultConnection": "Server=localhost,1433;Database=BookifyDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
```

### 4.3. تنظیم JWT Secret Key

کلید JWT باید حداقل 32 کاراکتر باشد. می‌توانید از ابزارهای آنلاین یا دستور زیر استفاده کنید:

```bash
# تولید کلید تصادفی در PowerShell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))

# یا در Bash
openssl rand -base64 64
```

### 4.4. دریافت کلیدهای Stripe (برای پرداخت)

1. به [Stripe Dashboard](https://dashboard.stripe.com/register) بروید
2. ثبت‌نام کنید (رایگان است)
3. به قسمت **Developers > API Keys** بروید
4. کلیدهای Test را کپی کنید:
   - **Publishable key**: شروع با `pk_test_`
   - **Secret key**: شروع با `sk_test_`
5. این کلیدها را در `appsettings.json` قرار دهید

> **توجه**: برای محیط Production از کلیدهای Live استفاده کنید.

### 4.5. ایجاد فایل appsettings.Development.json (اختیاری)

برای تنظیمات خاص Development:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "DetailedErrors": true
}
```

---

## 5. اجرای Migration

### 5.1. نصب ابزار EF Core (اگر نصب نیست)

```bash
# نصب dotnet-ef به صورت Global
dotnet tool install --global dotnet-ef

# بررسی نسخه
dotnet ef --version
```

### 5.2. اجرای Migration برای ایجاد دیتابیس

```bash
# ورود به root پروژه
cd /path/to/Bookify-Hotel-Reservation-System

# اجرای migration
dotnet ef database update --project Bookify.Infrastructure --startup-project Bookify.Api
```

این دستور:
1. به دیتابیس متصل می‌شود
2. دیتابیس را ایجاد می‌کند (اگر وجود ندارد)
3. تمام Migrationها را اجرا می‌کند
4. جداول، ستون‌ها، و روابط را ایجاد می‌کند

### 5.3. بررسی موفقیت‌آمیز بودن Migration

#### با استفاده از SSMS یا Azure Data Studio

```sql
USE BookifyDB;
GO

-- لیست تمام جداول
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- باید جداول زیر را ببینید:
-- AspNetUsers
-- AspNetRoles
-- AspNetUserRoles
-- Hotels
-- Rooms
-- RoomTypes
-- Bookings
-- Payments
-- و غیره...
```

#### با استفاده از dotnet ef

```bash
# مشاهده لیست Migrationهای اعمال شده
dotnet ef migrations list --project Bookify.Infrastructure --startup-project Bookify.Api
```

### 5.4. مشاهده یا ایجاد Migration جدید (برای توسعه‌دهندگان)

```bash
# ایجاد Migration جدید
dotnet ef migrations add MigrationName --project Bookify.Infrastructure --startup-project Bookify.Api

# حذف آخرین Migration (قبل از اعمال)
dotnet ef migrations remove --project Bookify.Infrastructure --startup-project Bookify.Api

# بازگشت به Migration خاص
dotnet ef database update MigrationName --project Bookify.Infrastructure --startup-project Bookify.Api
```

---

## 6. راه‌اندازی برنامه

### 6.1. اجرای برنامه از Command Line

```bash
# ورود به پوشه API
cd Bookify.Api

# اجرای برنامه
dotnet run
```

یا از root پروژه:

```bash
dotnet run --project Bookify.Api/Bookify.Api.csproj
```

### 6.2. اجرای برنامه از Visual Studio

1. فایل `Bookify.sln` را باز کنید
2. `Bookify.Api` را به عنوان Startup Project تنظیم کنید (راست کلیک > Set as Startup Project)
3. روی دکمه **Start (F5)** یا **Run Without Debugging (Ctrl+F5)** کلیک کنید

### 6.3. اجرای برنامه از VS Code

1. پوشه پروژه را در VS Code باز کنید
2. Terminal را باز کنید (`Ctrl+``)
3. دستور `dotnet run --project Bookify.Api` را اجرا کنید

یا از Run and Debug:
1. `F5` بزنید
2. از لیست `.NET Core Launch` را انتخاب کنید

### 6.4. بررسی اجرای موفق برنامه

پس از اجرا، خروجی مشابه زیر را خواهید دید:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5080
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7280
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 6.5. دسترسی به برنامه

برنامه در آدرس‌های زیر در دسترس است:

- **HTTP**: http://localhost:5080
- **HTTPS**: https://localhost:7280
- **Swagger UI**: https://localhost:7280/swagger
- **Health Check**: https://localhost:7280/health

### 6.6. تست اولیه با Swagger

1. مرورگر را باز کنید
2. به آدرس `https://localhost:7280/swagger` بروید
3. لیست تمام API Endpointها را خواهید دید
4. می‌توانید APIها را مستقیماً از Swagger تست کنید

---

## 7. رفع مشکلات رایج

### 7.1. خطای "Unable to connect to SQL Server"

**علت**: Connection String اشتباه یا SQL Server در حال اجرا نیست

**راه حل**:

```bash
# بررسی وضعیت SQL Server (ویندوز)
# در Services.msc دنبال SQL Server بگردید

# یا با PowerShell
Get-Service -Name MSSQL*

# Start کردن سرویس
Start-Service -Name "MSSQL$SQLEXPRESS"
```

برای Docker:
```bash
docker ps
docker start sqlserver
```

### 7.2. خطای "Login failed for user"

**علت**: اطلاعات کاربری در Connection String اشتباه است

**راه حل**:
- بررسی Username و Password در `appsettings.json`
- تست اتصال با SSMS
- برای SQL Express، حتماً `\SQLEXPRESS` را اضافه کنید

### 7.3. خطای "The certificate chain was issued by an authority that is not trusted"

**علت**: مشکل SSL Certificate در اتصال به SQL Server

**راه حل**:
به Connection String خود `TrustServerCertificate=True` اضافه کنید:

```json
"DefaultConnection": "Server=localhost;Database=BookifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 7.4. خطای "Could not find a part of the path"

**علت**: مسیر پروژه اشتباه است یا پوشه‌های لازم وجود ندارد

**راه حل**:
```bash
# اطمینان از ورود به پوشه درست
pwd  # یا cd برای مشاهده مسیر فعلی

# لیست فایل‌ها
ls -la

# باید Bookify.sln را ببینید
```

### 7.5. خطای "dotnet ef command not found"

**علت**: ابزار EF Core نصب نشده است

**راه حل**:
```bash
# نصب dotnet-ef
dotnet tool install --global dotnet-ef

# بروزرسانی PATH
# در ویندوز: restart terminal
# در Linux/Mac: source ~/.bashrc یا ~/.zshrc
```

### 7.6. خطای "Port 5080 or 7280 is already in use"

**علت**: برنامه دیگری از این پورت استفاده می‌کند

**راه حل**:

**گزینه 1**: پیدا کردن و بستن برنامه‌ای که از پورت استفاده می‌کند

```bash
# ویندوز
netstat -ano | findstr :5080
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5080
kill -9 <PID>
```

**گزینه 2**: تغییر پورت در `launchSettings.json`:

```json
{
  "profiles": {
    "Bookify.Api": {
      "applicationUrl": "https://localhost:7281;http://localhost:5081"
    }
  }
}
```

### 7.7. خطای "Migrations not found"

**علت**: Migrationها به درستی ایجاد نشده‌اند

**راه حل**:
```bash
# لیست Migrationها
dotnet ef migrations list --project Bookify.Infrastructure --startup-project Bookify.Api

# اگر خالی است، Migration جدید بسازید
dotnet ef migrations add InitialCreate --project Bookify.Infrastructure --startup-project Bookify.Api

# سپس update کنید
dotnet ef database update --project Bookify.Infrastructure --startup-project Bookify.Api
```

### 7.8. خطای "Unable to resolve service for type 'DbContext'"

**علت**: Dependency Injection به درستی تنظیم نشده است

**راه حل**:
بررسی کنید که در `Program.cs` خط زیر وجود دارد:

```csharp
builder.Services.AddDbContext<BookifyDbContext>(options =>
    options.UseSqlServer(connectionString));
```

### 7.9. خطای "JWT Token Invalid"

**علت**: تنظیمات JWT اشتباه یا Token منقضی شده است

**راه حل**:
- بررسی `Jwt:Key` در `appsettings.json` (حداقل 32 کاراکتر)
- بررسی `Jwt:Issuer` و `Jwt:Audience`
- Token جدید بگیرید (Login مجدد)

### 7.10. خطای "Stripe API Key Invalid"

**علت**: کلیدهای Stripe اشتباه یا منقضی شده‌اند

**راه حل**:
1. به [Stripe Dashboard](https://dashboard.stripe.com/apikeys) بروید
2. کلیدهای Test را کپی کنید
3. در `appsettings.json` جایگزین کنید
4. اطمینان حاصل کنید که از کلیدهای `test` استفاده می‌کنید نه `live`

### 7.11. خطای Build Failed

**علت**: پکیج‌ها نصب نشده‌اند یا خطای کد

**راه حل**:
```bash
# پاک کردن build قبلی
dotnet clean

# Restore مجدد
dotnet restore

# Build مجدد
dotnet build

# اگر همچنان خطا دارد، پاک کردن bin و obj
find . -name "bin" -o -name "obj" | xargs rm -rf
dotnet restore
dotnet build
```

### 7.12. دیتابیس خالی است (هیچ داده‌ای ندارد)

**راه حل**: اجرای Seed Data

کد Seeder در فایل `DatabaseSeeder.cs` و `SeedData.cs` موجود است. برای اجرا:

1. Breakpoint در `Program.cs` بگذارید
2. یا یک Endpoint برای Seed اضافه کنید:

```csharp
app.MapGet("/api/seed", async (BookifyDbContext context) =>
{
    await DatabaseSeeder.SeedAsync(context, serviceProvider);
    return Results.Ok("Data seeded successfully");
});
```

---

## 🎉 تبریک!

اگر تا اینجا آمده‌اید، پروژه Bookify با موفقیت نصب و راه‌اندازی شده است! 

### مراحل بعدی:

1. ✅ از طریق Swagger APIها را تست کنید
2. ✅ یک کاربر ثبت‌نام کنید
3. ✅ Login کنید و Token بگیرید
4. ✅ از APIهای محافظت شده استفاده کنید
5. ✅ Seed Data را اجرا کنید تا داده‌های نمونه داشته باشید

### مستندات بیشتر:

- [README اصلی](README.md)
- [README فارسی](README.fa.md)
- [راهنمای Docker](DOCKER.md)
- [راهنمای تست API](API_TESTING_GUIDE.md)

---

<div align="center">

**آیا با مشکلی مواجه شدید؟**

[📝 Issue جدید بسازید](https://github.com/kamikazeee1313-max/Bookify-Hotel-Reservation-System/issues)

**ساخته شده با ❤️ توسط تیم pentaRae**

</div>

</div>
