# Domain Modelleri

Bu klasör, uygulama genelinde kullanılan veri modellerini içerir.

Sınıf Bazında İnceleme
1. Attributes
İşlev: Dinamik anahtar-değer çiftleri ile kullanıcı veya kaynak bazında ek metadata tanımlamak için kullanılıyor.
Notlar:
EntityType enum’ı ile hangi tür varlık (User, Resource) için olduğu belirtilmiş; bu esneklik sağlasa da, ilişkisel bütünlük açısından daha spesifik tablolar veya türetme (inheritance) düşünülebilir.
2. AuditLog
İşlev: CRUD işlemlerine dair geçmiş kayıtları tutuyor.
Notlar:
Herhangi bir doğrudan ilişkilendirme olmaması, loglama amacı için doğru bir yaklaşım. Ancak loglama kayıtlarının büyüklüğünü göz önünde bulundurup, indeksleme ve arşivleme stratejileri planlanmalı.
3. Condition
İşlev: Zaman, IP, konum gibi koşulları tanımlıyor.
Notlar:
Expression alanının JSON veya DSL formatında olması, esnek bir yapı sunuyor. Doğrulama ve yorumlama katmanı eklenebilir.
4. IPRestrictions
İşlev: Belirli bir kaynak için IP erişim kısıtlamalarını yönetiyor.
Notlar:
Resource ile olan ilişkisel bağlantı açıkça belirtilmiş.
Audit alanları (CreatedBy, UpdatedBy, CreatedAt, UpdatedAt) tutarlı bir şekilde uygulanmış.
5. Modules & Pages
İşlev: Uygulamanın modüler yapısını ve modüllere ait sayfaları temsil ediyor.
Notlar:
“Modules” ve “Pages” isimlendirmesinde, tekil ve çoğul kullanım konusunda tutarlılık sağlanabilir (örneğin; Module, Page şeklinde).
Pages, Module ile ilişkilendirilmiş; bu, modüler yapıyı yönetmek için uygun.
6. Operation, Permission & PermissionScope
İşlev:
Operation: Uygulamadaki operasyonları tanımlıyor.
Permission: Bir operasyonun kaynak ile ilişkisini kurarak izin mantığını oluşturuyor.
PermissionScope: İzinler ve kapsam (scope) arasında ara tablo olarak görev yapıyor.
Notlar:
Her sınıfta audit bilgileri yer alıyor, bu da takip edilebilirliği artırıyor.
İlişki modelleri net; ancak Permission ile ilgili bazı alanlarda ilave açıklamalar (örneğin, hangi durumlarda izin verildiği vs.) dokümantasyonla desteklenebilir.
7. Policy, PolicyCondition, PolicyPermissions & PolicyVersion
İşlev:
Policy: Rol ve izinlerin bir araya getirilerek uygulandığı politika mantığını içeriyor.
PolicyCondition: Politikaya özel koşulları ilişkilendiriyor.
PolicyPermissions: Politika ve izinleri birbirine bağlıyor.
PolicyVersion: Politikanın versiyonlamasını, etkili tarihini ve yapılan değişiklikleri yönetiyor.
Notlar:
Policy’de “IsActive” ve “IsPublic” gibi alanlar veri tipi olarak string veya int tanımlanmış; burada mantıksal doğruluk (bool) veya enum kullanımı daha uygun olabilir.
Versiyonlama yaklaşımı, değişikliklerin izlenmesi açısından çok yararlı.
8. RefreshTokens & User (ve UserRoles)
İşlev:
RefreshTokens: Kullanıcı oturumunun uzun ömürlülüğünü sağlayacak token bilgisini tutuyor.
User: Temel kullanıcı bilgileri ve ilişkisel bağlantıları (roller, tokenlar) içeriyor.
Notlar:
Kullanıcı Id’sinin Guid olması, benzersizlik ve güvenlik açısından avantaj sağlar.
Email, NormalizedEmail gibi alanlarda benzersizlik ve indeksleme sağlanması önem arz eder.
UserRoles, çoktan çoğa ilişkiyi düzgün yönetiyor.
9. Role, RoleHierarchy & RolePolicy
İşlev:
Role: Kullanıcı rollerini tanımlıyor.
RoleHierarchy: Roller arasında hiyerarşik ilişki kurarak ebeveyn-çocuk yapısı sağlıyor.
RolePolicy: Roller ile politikalar arasındaki ilişkiyi yönetiyor.
Notlar:
Rol bazlı yetkilendirme sistemi, hiyerarşik yapı ile genişletilmiş.
RolePolicy içerisindeki tanımlanmamış sabit ("private const string V = 'Policy'") temizlenebilir veya açıklanabilir.
Rol ilişkilerinde, navigasyon ve yönlendirme işlemlerinde ekstra kontrollerin yapılması gerekebilir.
10. Scope
İşlev:
Kaynak tabanlı filtreleme ve erişim kapsamlarını tanımlıyor.
Notlar:
Resource ile olan bağlantı, hangi kaynak üzerinde filtre uygulanacağını netleştiriyor.
İlişkisel Bağlantılar ve Veri Tutarlılığı
Yabancı Anahtarlar:
[ForeignKey] öznitelikleri doğru şekilde uygulanmış. Her ilişki (örneğin; IPRestrictions-Resource, Pages-Modules, vb.) net olarak tanımlanmış.
Navigasyon Özellikleri:
Koleksiyonlar (ör. UserRoles, RefreshTokens) varsayılan olarak yeni listelerle initialize edilmiş. Bu, null referans hatalarını önlemek açısından iyi.
Çoktan Çoğa İlişkiler:
Rol, kullanıcı, politika ve izin arasında ara tablolar kullanılarak ilişkiler doğru şekilde modellenmiş.
Bu yaklaşımla, genişleyen ve karmaşık yetkilendirme senaryolarında esneklik sağlanıyor.
İsimlendirme ve Tutarlılık
İsimlendirme:
Genel olarak isimlendirme anlaşılır; ancak bazı sınıfların isimlendirilmesinde (örneğin, "Modules" ve "Pages") tekil/çoğul karışıklığı giderilebilir.
Veri Tipleri:
Boolean değerler için “IsActive” gibi alanların int yerine bool olarak tanımlanması okunabilirliği ve tutarlılığı artırabilir.
“IsPublic” alanının string olarak tanımlanması, potansiyel hata kaynağı olabilir; mantıksal ifade için bool tercih edilebilir.
Audit Alanları:
Birçok sınıfta CreatedBy, CreatedAt, UpdatedBy, UpdatedAt gibi audit bilgileri mevcut. Tutarlı kullanım, ileride raporlama ve hata ayıklama açısından büyük avantaj sağlar.
Öneriler ve İyileştirmeler
Tekil/Çoğul İsimlendirme:

Entity isimlerinde (örneğin, Module yerine Modules yerine “Module”, Page yerine “Page”) tekil kullanımı tercih ederek ORM mapping’inde tutarlılığı artırabilirsiniz.
Veri Tipi Düzenlemeleri:

“IsActive” ve “IsPublic” gibi alanlarda mantıksal (boolean) veri tipi kullanımı, veritabanı ve uygulama kodunda daha açık bir anlam sağlar.
Enum ve Sınıf İlişkileri:

Attributes sınıfında EntityType enum’ı kullanılması esneklik sağlasa da, veritabanı düzeyinde yabancı anahtar ilişkileriyle ayrı tablolar üzerinden kalıtım (inheritance) modeli de düşünülebilir.
İndeksleme ve Performans:

Özellikle kullanıcı ve email gibi alanlarda, benzersizlik ve arama performansı için indeksler oluşturulmalı.
Audit log tablosu için de zaman damgası ve işlem tiplerine göre indeksleme planlanabilir.
Fluent API Kullanımı:

İlişkileri daha detaylı kontrol etmek için, Entity Framework’ün Fluent API’siyle cascade delete, ilişkisel davranış ve konfigürasyonlar netleştirilebilir.
Temiz Kod Yaklaşımı:

Kullanılmayan veya anlamsız sabitler (örneğin, RolePolicy içindeki “private const string V”) temizlenmeli.
Null güvenliği (C# 8 ve üzeri) için nullable referans tipleri ve ilgili kontroller eklenebilir.
Sonuç
Genel olarak, tasarımınız karmaşık yetkilendirme, audit, rol-hiyerarşi ve dinamik veri tanımlama ihtiyaçlarını karşılayacak şekilde detaylandırılmış. İlişkisel bağlar doğru şekilde modellenmiş; ancak isimlendirme, veri tipi tutarlılığı ve ek indeksleme/konfigürasyon detayları üzerinde küçük iyileştirmeler yapılarak sistemin daha okunabilir, yönetilebilir ve performanslı hale getirilmesi mümkün.

Bu detaylandırılmış yapı, ilerleyen dönemlerde ihtiyaç duyulabilecek genişleme ve modifikasyonlara da oldukça uygun gözüküyor.


