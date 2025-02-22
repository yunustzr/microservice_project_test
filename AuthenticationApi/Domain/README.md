# Domain Modelleri

Bu klasör, uygulama genelinde kullanılan veri modellerini içerir.

### 📄 `User.cs`
- Kullanıcı entity'si (Veritabanı tablosu ile eşleşir).
- **Özellikler**:
  - `Id`: Benzersiz kullanıcı ID'si.
  - `Email`: Kullanıcı e-posta adresi.
  - `PasswordHash`: BCrypt ile hash'lenmiş şifre.

### 📄 `LoginRequest.cs`
- Login endpoint'i için gelen istek modeli.