using Microsoft.AspNetCore.Mvc;
using DotnetPN.Models;
using DotnetPN.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetPN.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TodoController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
    {
        var items = await _unitOfWork.TodoItems.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> Get(int id)
    {
        var item = await _unitOfWork.TodoItems.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(TodoItem todo)
    {
        await _unitOfWork.TodoItems.AddAsync(todo);
        await _unitOfWork.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TodoItem todo)
    {
        if (id != todo.Id) return BadRequest();
        _unitOfWork.TodoItems.Update(todo);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _unitOfWork.TodoItems.GetByIdAsync(id);
        if (todo == null) return NotFound();
        _unitOfWork.TodoItems.Delete(todo);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}
