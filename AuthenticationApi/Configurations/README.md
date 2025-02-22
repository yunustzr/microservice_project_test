
---

### **Configurations/README.md**

```markdown
# Konfigürasyon Yönetimi

Bu klasör, uygulama konfigürasyonlarını içeren sınıfları barındırır.

### 📄 `LdapConfig.cs`
- LDAP bağlantı bilgilerini tutar (Sunucu adresi, port, base DN).
- **Özellikler**:
  - `Server`: LDAP sunucu adresi.
  - `Port`: LDAP port numarası.
  - `BaseDn`: LDAP arama tabanı (Örn: `dc=example,dc=com`).