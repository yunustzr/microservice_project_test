# Veritabanı ve Altyapı

Bu klasör, veritabanı bağlantısı ve repository pattern implementasyonlarını içerir.

### 📄 `AppDbContext.cs`
- Entity Framework Core DbContext sınıfı.
- **Özellikler**:
  - `Users`: Kullanıcılar için DbSet.

### 📄 `UserRepository.cs`
- Veritabanı işlemlerini soyutlayan repository sınıfı.
- **Metodlar**:
  - `GetUserByEmailAsync`: E-posta ile kullanıcı sorgulama.