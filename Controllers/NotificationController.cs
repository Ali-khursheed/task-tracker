using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Repositories;
using TaskTracker.Interfaces;
namespace TaskTracker.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class NotificationController: ControllerBase
  {
    private readonly INotificationRepository _notifRepo;

    public NotificationController(NotificationRepository notifRepo)
    {
      _notifRepo=notifRepo;
    }

    private int GetCurrentUserId()
    {
      var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return int.Parse(claim!);
    }



    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
      var userId = GetCurrentUserId();
      var notifications = await _notifRepo.GetByUserIdAsync(userId);
      return Ok(notifications);
    }


    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
      var userId = GetCurrentUserId();
      var count = await _notifRepo.GetUnreadCountAsync(userId);
      return Ok(new { count });
    }


    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
      var userId = GetCurrentUserId();
      await _notifRepo.MarkAsReadAsync(id, userId);
      return Ok(new { message = "Marked as read" });
    }







  }
}
