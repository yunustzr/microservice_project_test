# API Kontrolleri

Bu klasör, HTTP isteklerini yöneten MVC Controller'larını içerir.

### 📄 `AuthController.cs`
- **Endpoint'ler**:
  - `POST /api/auth/login`: LDAP ile kimlik doğrulama ve JWT token üretimi.
  - `POST /api/auth/register`: Kullanıcı kaydı (Opsiyonel).
- **Bağımlılıklar**:
  - `ILdapService`: LDAP doğrulama servisi.
  - `IConfiguration`: JWT ayarları için.