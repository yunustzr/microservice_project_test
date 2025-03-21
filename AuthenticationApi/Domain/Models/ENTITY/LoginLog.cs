using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Models.ENTITY;


public class LoginLog
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime LoginTime { get; set; }
    public string IPAddress { get; set; }
    public string DeviceInfo { get; set; }
    public bool IsSuccessful { get; set; }
    public string FailureReason { get; set; }
    public DateTime LogoutTime {get;set;}
}

