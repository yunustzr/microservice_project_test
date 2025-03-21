# Authentication API

Bu proje, LDAP tabanlı kimlik doğrulama ve JWT token yönetimi içeren bir mikroservis mimarisidir. 
Aşağıda projenin klasör yapısı ve bileşenlerinin açıklamaları bulunmaktadır.

---

## 📂 Klasör Yapısı ve Açıklamalar

| Klasör/Alt Klasör                                |                   Açıklama                                                |
|--------------------------------------------------|---------------------------------------------------------------------------|
| [**Configurations**](./Configurations/README.md) | Uygulama konfigürasyon sınıfları (LDAP, JWT vb.).                         |
| [**Controllers**](./Controllers/README.md)       | API endpoint'lerini içeren MVC Controller'ları.                           |
| [**Domain**](./Domain/README.md)                 | Entity'ler, DTO'lar ve domain modelleri.                                  |
| [**Infrastructure**](./Infrastructure/README.md) | Veritabanı bağlantısı (EF Core) ve repository pattern implementasyonları. |
| [**Services**](./Services/README.md)             | İş mantığı (business logic) ve harici servis entegrasyonları.             |

---

## 🚀 Hızlı Başlangıç


### Migration İşlemi
* DBContext.cs clasındaki örneklemeye bakarak connection stringde belirtilen database'e tabloları oluşturur.
dotnet ef migrations add InitialCreate
dotnet ef database update

* Tekrar aynı migration ile işlem yapmak için;
dotnet ef migrations remove -f
dotnet ef migrations add InitialCreate
dotnet ef database update

*NOT: database'de aynı tablo var diye hata alırsanız database drop edip yeniden oluşturulmalıdır.
### Gereksinimler
- .NET 8 SDK
- Docker (Opsiyonel: LDAP test ortamı için)
- PostgreSQL veya tercih edilen bir veritabanı

### Kurulum
1. **Projeyi klonlayın**:
   ```bash
   git clone https://github.com/your-repo/AuthenticationApi.git
   cd AuthenticationApi


### Projeye Referans Ekleme
cd AuthenticationApi
dotnet add reference ..\SharedLibrary\SharedLibrary.csproj