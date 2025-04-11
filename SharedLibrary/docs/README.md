# Paylaşılan Kütüphane

Bu kütüphane, projedeki mikroservisler arasında paylaşılan ortak kod, yardımcı programlar ve bileşenleri içerir.

## İçerik

- Ortak Modeller
- Veri Transfer Nesneleri (DTO'lar)
- Yardımcı Programlar
- Uzantılar
- Sabitler
- Özel İstisnalar
- Paylaşılan Yapılandırmalar

## Kullanım

Bu paylaşılan kütüphaneyi mikroservis projenizde kullanmak için:

1. Bu projeye referans ekleyin
2. Gerekli ad alanlarını içe aktarın
3. Paylaşılan bileşenleri gerektiği gibi kullanın

## Yapı

```
SharedLibrary/
├── Models/
├── DTOs/
├── Utils/
├── Extensions/
├── Constants/
├── Exceptions/
└── Configurations/
```

## Bağımlılıklar

- .NET 7.0+
- Diğer temel bağımlılıklar burada listelenecek

## Katkıda Bulunma

Yeni paylaşılan bileşenler eklerken:

1. Bileşenin gerçekten birden fazla servis arasında paylaşıldığından emin olun
2. Mevcut kod yapısını ve kurallarını takip edin
3. Uygun dokümantasyon ekleyin
4. Gerekirse bu README'yi güncelleyin

## Geliştiriciler

- Geliştiricileri burada listeleyin

## Lisans

Lisansınızı burada belirtin