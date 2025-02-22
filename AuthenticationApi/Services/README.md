# Servisler ve İş Mantığı

Bu klasör, uygulamanın iş mantığını ve harici servis entegrasyonlarını içerir.

### 📄 `LdapService.cs`
- LDAP sunucusu ile kimlik doğrulama yapar.
- **Metodlar**:
  - `Authenticate`: Kullanıcı adı/şifre doğrulaması.

### 📄 `AuthenticationService.cs`
- JWT token üretimi ve doğrulama işlemleri.
- **Bağımlılıklar**:
  - `ILdapService`
  - `IUserRepository`

### 📄 `PasswordHasher.cs`
- Şifre hash'leme ve doğrulama için BCrypt implementasyonu.