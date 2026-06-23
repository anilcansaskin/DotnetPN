using System;
using System.ComponentModel.DataAnnotations;

namespace DotnetPN.Models;

public class Device
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string DeviceToken { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
