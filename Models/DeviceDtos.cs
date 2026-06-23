using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotnetPN.Models;

public class RegisterDeviceRequest
{
    [Required]
    public string DeviceToken { get; set; } = string.Empty;
    
    public string? UserId { get; set; }
}

public class UnregisterDeviceRequest
{
    [Required]
    public string DeviceToken { get; set; } = string.Empty;
}

public class SendNotificationRequest
{
    public string? Token { get; set; }
    
    public string? UserId { get; set; }
    
    public bool SendToAll { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    public Dictionary<string, string>? Data { get; set; }
}
