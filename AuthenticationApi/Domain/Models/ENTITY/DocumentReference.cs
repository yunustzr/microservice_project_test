using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY;

public class DocumentReference
{
    [Key]
    public int Id { get; set; }
    public string DocumentName { get; set; }  // Evrak adı veya başlığı
    public string DocumentUrl { get; set; }     // Evrakın erişim linki
    public string Description { get; set; }     // Evrakın açıklaması veya kısa özeti
}
