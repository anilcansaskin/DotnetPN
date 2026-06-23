using Microsoft.AspNetCore.Mvc;
using DotnetPN.Models;
using DotnetPN.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DotnetPN.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFcmService _fcmService;

    public DevicesController(IUnitOfWork unitOfWork, IFcmService fcmService)
    {
        _unitOfWork = unitOfWork;
        _fcmService = fcmService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDeviceRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingDevice = await _unitOfWork.Devices.GetByTokenAsync(request.DeviceToken);
        if (existingDevice != null)
        {
            existingDevice.UserId = request.UserId;
            existingDevice.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Devices.Update(existingDevice);
        }
        else
        {
            var newDevice = new Device
            {
                DeviceToken = request.DeviceToken,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Devices.AddAsync(newDevice);
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok(new { Message = "Device registered successfully." });
    }

    [HttpPost("unregister")]
    public async Task<IActionResult> Unregister([FromBody] UnregisterDeviceRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var device = await _unitOfWork.Devices.GetByTokenAsync(request.DeviceToken);
        if (device == null)
        {
            return NotFound(new { Message = "Device token not found." });
        }

        _unitOfWork.Devices.Delete(device);
        await _unitOfWork.SaveChangesAsync();
        return Ok(new { Message = "Device unregistered successfully." });
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (!string.IsNullOrEmpty(request.Token))
        {
            var success = await _fcmService.SendNotificationAsync(request.Token, request.Title, request.Body, request.Data);
            if (success)
            {
                return Ok(new { Message = "Notification sent successfully." });
            }
            return StatusCode(500, new { Message = "Failed to send notification. Check server logs." });
        }

        if (!string.IsNullOrEmpty(request.UserId))
        {
            var devices = await _unitOfWork.Devices.GetByUserIdAsync(request.UserId);
            if (devices.Count == 0)
            {
                return NotFound(new { Message = $"No registered devices found for user {request.UserId}." });
            }

            var tokens = devices.Select(d => d.DeviceToken);
            var successCount = await _fcmService.SendNotificationToAllAsync(tokens, request.Title, request.Body, request.Data);
            return Ok(new { Message = $"Notification sent to {successCount} out of {devices.Count} devices." });
        }

        if (request.SendToAll)
        {
            var devices = await _unitOfWork.Devices.GetAllAsync();
            if (devices.Count == 0)
            {
                return NotFound(new { Message = "No devices registered in the system." });
            }

            var tokens = devices.Select(d => d.DeviceToken);
            var successCount = await _fcmService.SendNotificationToAllAsync(tokens, request.Title, request.Body, request.Data);
            return Ok(new { Message = $"Notification sent to {successCount} out of {devices.Count} devices." });
        }

        return BadRequest(new { Message = "You must provide either a target Token, UserId, or set SendToAll to true." });
    }
}
