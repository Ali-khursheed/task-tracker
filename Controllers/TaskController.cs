
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Models.Dtos.Tasks;
using TaskTracker.Services;
namespace TaskTracker.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class TaskController:ControllerBase
  {
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
      _taskService=taskService;
    }

    // Helper — reads userId from JWT token claims
    // This is why we embedded userId in the token during login
    private int GetCurrentUserId()
    {
      var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return int.Parse(claim!);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
      var assignerId = GetCurrentUserId();
      var taskId = await _taskService.CreateTaskAsync(request, assignerId);
      return Ok(new { taskId, message = "Task created successfully" });
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
      var task = await _taskService.GetByIdAsync(id);
      if(task==null) return NotFound();
      return Ok(task);
    }

    [HttpGet("assigned-to-me")]
    public async Task<IActionResult> GetAssignedToMe()
    {
      var userId = GetCurrentUserId();
      var tasks = await _taskService.GetAssignedToMeAsync(userId);
      return Ok(tasks);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusRequest request)
    {
      var userId = GetCurrentUserId();
      var (success, message)=await _taskService.UpdateStatusAsync(id, request.NewStatus, userId);

      if(!success) return BadRequest(new { message });
      return Ok(new { message });
    }


  }
}
