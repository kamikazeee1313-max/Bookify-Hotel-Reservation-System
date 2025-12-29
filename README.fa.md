# 🏨 Bookify - سیستم رزرو هتل

<div dir="rtl">

## 📖 درباره پروژه

**Bookify** یک سیستم جامع و حرفه‌ای برای مدیریت رزرو هتل است که با استفاده از تکنولوژی‌های مدرن ASP.NET Core طراحی و پیاده‌سازی شده است. این سیستم به مشتریان امکان جستجو و رزرو اتاق‌های هتل را می‌دهد و همچنین پنل مدیریتی قدرتمندی برای مدیران هتل فراهم می‌کند.

## ✨ ویژگی‌های اصلی

### 🏢 مدیریت هتل‌ها
- ➕ افزودن هتل جدید
- ✏️ ویرایش اطلاعات هتل
- 🗑️ حذف هتل
- 📋 نمایش لیست هتل‌ها با جزئیات کامل

### 🛏️ مدیریت اتاق‌ها
- افزودن و ویرایش اتاق‌ها
- قیمت‌گذاری پویا برای هر اتاق
- مدیریت موجودی و وضعیت اتاق‌ها
- دسته‌بندی اتاق‌ها بر اساس نوع (Suite، Deluxe، Standard)

### 📅 سیستم رزرو
- رزرو آنلاین اتاق با تاریخ‌های مشخص
- بررسی خودکار موجودی اتاق‌ها
- محاسبه خودکار قیمت بر اساس تعداد شب‌ها
- لغو رزرو با بازگرداندن موجودی
- مشاهده تاریخچه رزروها

### 💳 پرداخت امن
- پرداخت آنلاین از طریق Stripe
- پشتیبانی از کارت‌های اعتباری
- ثبت و پیگیری تراکنش‌ها

### 👥 مدیریت کاربران
- ثبت‌نام و احراز هویت کاربران
- نقش‌های مختلف: Admin (مدیر) و Employee (کارمند)
- مدیریت پروفایل کاربری
- تغییر رمز عبور

### 🔐 امنیت
- احراز هویت مبتنی بر JWT Token
- رمزنگاری رمز عبور
- دسترسی محدود به API‌ها بر اساس نقش کاربر
- اعتبارسنجی داده‌های ورودی

### 📊 پنل مدیریت
- داشبورد جامع برای مدیران
- گزارش‌گیری از رزروها و پرداخت‌ها
- مدیریت کاربران و نقش‌ها
- آمار و ارقام سیستم

## 🛠️ تکنولوژی‌های استفاده شده

### Backend
- **Framework**: ASP.NET Core 10.0 Web API
- **Language**: C# 12
- **ORM**: Entity Framework Core
- **Database**: SQL Server 2019+
- **Authentication**: JWT (JSON Web Token)
- **Payment Gateway**: Stripe API
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

### Frontend
- **Engine**: Razor Pages
- **UI Framework**: Bootstrap 5
- **JavaScript Library**: jQuery
- **Notifications**: Toastr.js
- **Styling**: CSS3

### معماری و الگوهای طراحی
- **Architecture**: Clean Architecture (N-Tier)
- **Patterns**: 
  - Repository Pattern
  - Unit of Work Pattern
  - Dependency Injection
  - CQRS (با MediatR)
- **Layering**:
  - `Bookify.Domain`: موجودیت‌ها و منطق دامنه
  - `Bookify.Application`: سرویس‌ها و DTOها
  - `Bookify.Infrastructure`: دسترسی به داده و سرویس‌های خارجی
  - `Bookify.Api`: کنترلرها و API Endpoints
  - `Bookify.Shared`: توابع و کلاس‌های مشترک

## 📋 پیشنیازها

قبل از نصب و راه‌اندازی، اطمینان حاصل کنید که موارد زیر را نصب کرده‌اید:

### نرم‌افزارهای مورد نیاز
- ✅ [.NET SDK 10.0](https://dotnet.microsoft.com/download) یا بالاتر
- ✅ [SQL Server 2019+](https://www.microsoft.com/sql-server) (یا SQL Server Express)
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) یا [VS Code](https://code.visualstudio.com/)
- ✅ [Git](https://git-scm.com/)

### اختیاری
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)
- [Postman](https://www.postman.com/) برای تست API
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (برای اجرای با Docker)

## 🚀 نصب و راه‌اندازی

### 1️⃣ دانلود پروژه

```bash
# Clone کردن پروژه از GitHub
git clone https://github.com/kamikazeee1313-max/Bookify-Hotel-Reservation-System.git

# ورود به پوشه پروژه
cd Bookify-Hotel-Reservation-System
```

### 2️⃣ بازیابی پکیج‌ها

```bash
# بازیابی تمام پکیج‌های NuGet
dotnet restore
```

### 3️⃣ تنظیم Connection String

یک فایل `appsettings.json` در پوشه `Bookify.Api` ایجاد کنید (در صورتی که وجود ندارد):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookifyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWTMustBeAtLeast32Characters!",
    "Issuer": "BookifyApi",
    "Audience": "BookifyClient",
    "DurationInMinutes": 60
  },
  "Stripe": {
    "SecretKey": "sk_test_your_stripe_secret_key",
    "PublishableKey": "pk_test_your_stripe_publishable_key"
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

> **توجه**: مقادیر `Jwt:Key` و کلیدهای Stripe را با مقادیر واقعی خود جایگزین کنید.

### 4️⃣ ایجاد دیتابیس

```bash
# اجرای Migration برای ایجاد دیتابیس
dotnet ef database update --project Bookify.Infrastructure --startup-project Bookify.Api

# یا اگر dotnet ef نصب نیست:
dotnet tool install --global dotnet-ef
dotnet ef database update --project Bookify.Infrastructure --startup-project Bookify.Api
```

### 5️⃣ اجرای پروژه

```bash
# ورود به پوشه API
cd Bookify.Api

# اجرای پروژه
dotnet run
```

پس از اجرا، برنامه در آدرس‌های زیر در دسترس خواهد بود:
- HTTP: `http://localhost:5080`
- HTTPS: `https://localhost:7280`
- Swagger UI: `https://localhost:7280/swagger`

### 6️⃣ Seed Data (اختیاری)

برای افزودن داده‌های نمونه به دیتابیس، می‌توانید از کد Seeder استفاده کنید. این داده‌ها شامل کاربران، نقش‌ها، هتل‌ها و اتاق‌های نمونه هستند.

## 📁 ساختار پروژه

```
Bookify-Hotel-Reservation-System/
│
├── Bookify.Api/                  # لایه API و Controllers
│   ├── Controllers/              # کنترلرهای Web API
│   ├── HealthChecks/            # Health Check برای بررسی سلامت سیستم
│   ├── Program.cs               # نقطه شروع برنامه
│   └── appsettings.json         # تنظیمات برنامه
│
├── Bookify.Application/          # لایه Application/Business Logic
│   ├── Dtos/                    # Data Transfer Objects
│   ├── Interfaces/              # Interfaceهای سرویس‌ها
│   ├── Services/                # پیاده‌سازی سرویس‌ها
│   ├── Mappings/                # AutoMapper Profiles
│   └── Rooms/                   # منطق مربوط به اتاق‌ها
│
├── Bookify.Domain/               # لایه Domain (موجودیت‌ها)
│   ├── Entities/                # کلاس‌های Entity
│   ├── Enum/                    # Enumerationها
│   └── Exceptions/              # استثناهای سفارشی
│
├── Bookify.Infrastructure/       # لایه Infrastructure (Data Access)
│   ├── Data/
│   │   ├── Context/            # DbContext
│   │   ├── Repositories/       # Repository Implementations
│   │   ├── UnitOfWork/         # Unit of Work Pattern
│   │   └── AdminServices/      # سرویس‌های مدیریتی
│   ├── Payments/               # پیاده‌سازی Stripe Payment
│   ├── DatabaseSeeder.cs       # Seed Data
│   └── SeedData.cs
│
├── Bookify.Shared/               # کلاس‌های مشترک
│   ├── Exceptions/              # Exception Handlers
│   └── Helpers/                 # Helper Classes
│
├── Bookify.Tests/                # تست‌های واحد
│   ├── BookingServiceTests.cs
│   ├── RoomServiceTests.cs
│   └── UserServiceTests.cs
│
└── Bookify.sln                   # Solution File
```

## 🔌 API Endpoints

### 🔐 Authentication (`/api/auth`)

| Method | Endpoint | توضیحات |
|--------|----------|---------|
| POST | `/api/auth/register` | ثبت‌نام کاربر جدید |
| POST | `/api/auth/login` | ورود و دریافت Token |
| POST | `/api/auth/refresh-token` | تمدید Token |

### 👤 Profile (`/api/profile`)

| Method | Endpoint | توضیحات |
|--------|----------|---------|
| GET | `/api/profile` | دریافت اطلاعات پروفایل |
| PUT | `/api/profile` | بروزرسانی پروفایل |
| POST | `/api/profile/change-password` | تغییر رمز عبور |

### 🛏️ Rooms (`/api/rooms`)

| Method | Endpoint | توضیحات |
|--------|----------|---------|
| GET | `/api/rooms` | لیست تمام اتاق‌های موجود |
| GET | `/api/rooms/{id}` | جزئیات یک اتاق |
| GET | `/api/rooms/search` | جستجوی اتاق‌ها |

### 📅 Bookings (`/api/bookings`)

| Method | Endpoint | توضیحات |
|--------|----------|---------|
| POST | `/api/bookings` | ایجاد رزرو جدید |
| GET | `/api/bookings` | لیست رزروهای کاربر |
| GET | `/api/bookings/{id}` | جزئیات یک رزرو |
| DELETE | `/api/bookings/{id}` | لغو رزرو |

### 💳 Payment (`/api/payment`)

| Method | Endpoint | توضیحات |
|--------|----------|---------|
| POST | `/api/payment/create-intent` | ایجاد Payment Intent |
| POST | `/api/payment/confirm` | تایید پرداخت |

### 🔧 Admin Endpoints

#### مدیریت اتاق‌ها (`/api/admin/rooms`)
| Method | Endpoint | توضیحات |
|--------|----------|---------|
| POST | `/api/admin/rooms` | افزودن اتاق جدید |
| PUT | `/api/admin/rooms/{id}` | ویرایش اتاق |
| DELETE | `/api/admin/rooms/{id}` | حذف اتاق |

#### مدیریت رزروها (`/api/admin/bookings`)
| Method | Endpoint | توضیحات |
|--------|----------|---------|
| GET | `/api/admin/bookings` | لیست تمام رزروها |
| PUT | `/api/admin/bookings/{id}/status` | تغییر وضعیت رزرو |

#### مدیریت نوع اتاق‌ها (`/api/admin/roomtypes`)
| Method | Endpoint | توضیحات |
|--------|----------|---------|
| GET | `/api/admin/roomtypes` | لیست نوع اتاق‌ها |
| POST | `/api/admin/roomtypes` | افزودن نوع اتاق جدید |

### ❤️ Health Check (`/health`)

| Method | Endpoint | توضیحات |
|--------|----------|---------|
| GET | `/health` | بررسی سلامت سیستم و دیتابیس |

## 🧪 نحوه تست

### استفاده از Swagger UI

1. برنامه را اجرا کنید
2. به آدرس `https://localhost:7280/swagger` بروید
3. برای استفاده از APIهای محافظت شده:
   - ابتدا از `/api/auth/register` یک کاربر ایجاد کنید
   - سپس از `/api/auth/login` وارد شوید و Token دریافت کنید
   - روی دکمه "Authorize" کلیک کنید
   - Token را به صورت `Bearer {your-token}` وارد کنید

### استفاده از Postman

1. مجموعه APIهای Bookify را از پوشه پروژه import کنید
2. متغیرهای محیطی را تنظیم کنید:
   - `base_url`: `http://localhost:5080`
   - `token`: (پس از Login دریافت می‌شود)
3. ابتدا Register و Login کنید
4. Token دریافتی را در Header تمام درخواست‌ها قرار دهید

### مثال تست با cURL

```bash
# ثبت‌نام
curl -X POST "http://localhost:5080/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "علی",
    "lastName": "محمدی",
    "email": "ali@example.com",
    "password": "Password123!",
    "phoneNumber": "09123456789"
  }'

# ورود
curl -X POST "http://localhost:5080/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "ali@example.com",
    "password": "Password123!"
  }'

# دریافت لیست اتاق‌ها (با Token)
curl -X GET "http://localhost:5080/api/rooms" \
  -H "Authorization: Bearer {your-token}"
```

## 🤝 مشارکت در پروژه

ما از مشارکت شما استقبال می‌کنیم! برای مشارکت در پروژه:

1. پروژه را Fork کنید
2. یک Branch جدید برای ویژگی خود ایجاد کنید:
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. تغییرات خود را Commit کنید:
   ```bash
   git commit -m 'Add some AmazingFeature'
   ```
4. Branch را Push کنید:
   ```bash
   git push origin feature/AmazingFeature
   ```
5. یک Pull Request ایجاد کنید

### استانداردهای کد

- از Clean Code Principles پیروی کنید
- کامنت‌های مفید بنویسید
- از نام‌گذاری معنادار استفاده کنید
- تست‌های واحد برای کدهای جدید بنویسید

## 👥 تیم توسعه (pentaRae)

- **أحمد خالد (Ahmad Khaled)** - قائد الفريق / مدير المشروع
- **بسنت خطاب (Bassant Khattab)** - Frontend & Backend Developer
- **سلمي سمير (Salma Samir)** - Frontend & Backend Developer
- **عبدالله إبراهيم (Abdullah Ibrahim)** - Frontend & Backend Developer
- **مهاب إسلام (Mahab Islam)** - Database & DevOps
- **نهى شريف (Noha Sharif)** - Documentation & QA

## 📚 مستندات بیشتر

برای اطلاعات تکمیلی، فایل‌های زیر را مطالعه کنید:

- [راهنمای نصب تفصیلی (INSTALLATION.fa.md)](INSTALLATION.fa.md)
- [راهنمای Docker (DOCKER.md)](DOCKER.md)
- [راهنمای تست API (API_TESTING_GUIDE.md)](API_TESTING_GUIDE.md)
- [راهنمای سریع تست (QUICK_START_TESTING.md)](QUICK_START_TESTING.md)

## 📞 پشتیبانی

اگر با مشکلی مواجه شدید یا سوالی دارید:

- 📧 ایمیل: [support@bookify.com](mailto:support@bookify.com)
- 🐛 گزارش باگ: [GitHub Issues](https://github.com/kamikazeee1313-max/Bookify-Hotel-Reservation-System/issues)
- 📖 مستندات: [GitHub Wiki](https://github.com/kamikazeee1313-max/Bookify-Hotel-Reservation-System/wiki)

## 📄 لایسنس

این پروژه تحت لایسنس DEPI منتشر شده است.

## 🙏 تشکر ویژه

- از **استاد Hesham Mohamed** بابت راهنمایی‌ها و آموزش‌های ارزشمند
- از تمامی اعضای تیم **pentaRae** بابت تلاش و همکاری
- از جامعه ASP.NET Core بابت منابع و ابزارهای عالی

---

<div align="center">

**ساخته شده با ❤️ توسط تیم pentaRae**

[⬆ بازگشت به بالا](#-bookify---سیستم-رزرو-هتل)

</div>

</div>
