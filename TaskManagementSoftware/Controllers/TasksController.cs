using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSoftware.Data;
using TaskManagementSoftware.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSoftware.Helpers;

namespace TaskManagementSoftware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost("ClearData")]
        public async Task<IActionResult> ClearData()
        {
            _context.TaskMaterialUsages.RemoveRange(_context.TaskMaterialUsages);

            _context.Tasks.RemoveRange(_context.Tasks);

            _context.Materials.RemoveRange(_context.Materials);

            await _context.SaveChangesAsync();
            return Ok("All data cleared.");
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetTasks()
        {
            return await _context.Tasks
                .Include(t => t.TaskMaterialUsages)
                .ThenInclude(tmu => tmu.Material)
                .ToListAsync();
        }

        [HttpGet("TaskMaterialUsages")]
        public async Task<ActionResult<IEnumerable<TaskMaterialUsageDto>>> GetTaskMaterialUsages()
        {
            var taskMaterialUsages = await _context.TaskMaterialUsages
                .Include(tmu => tmu.Task) 
                .Include(tmu => tmu.Material) 
                .Select(tmu => new TaskMaterialUsageDto
                {
                    TaskName = tmu.Task.Name, 
                    MaterialName = tmu.Material.PartNumber,
                    Amount = tmu.Amount,
                    UnitOfMeasurement = tmu.UnitOfMeasurement
                })
                .ToListAsync();

            return Ok(taskMaterialUsages);
        }


        [HttpGet("{name}")]
        public async Task<ActionResult<Models.Task>> GetTask(string name)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskMaterialUsages)
                .ThenInclude(tmu => tmu.Material)
                .FirstOrDefaultAsync(t => t.Name == name);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }


        [HttpPost]
        public async Task<ActionResult<Models.Task>> PostTask([FromBody] TaskDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest("Task data is required.");
            }

            if (string.IsNullOrEmpty(taskDto.Name) || taskDto.TotalDuration <= 0)
            {
                return BadRequest("Invalid task data.");
            }
            var tasks = new Models.Task
            {
                ID = Guid.NewGuid(),
                Name = taskDto.Name,
                Description = taskDto.Description,
                TotalDuration = taskDto.TotalDuration
            };
            _context.Tasks.Add(tasks);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { id = tasks.ID }, tasks);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(Guid id, [FromBody] Models.Task task)
        {
            if (id != task.ID)
            {
                return BadRequest("Task ID mismatch.");
            }

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(Guid id)
        {
            return _context.Tasks.Any(e => e.ID == id);
        }

        [HttpPost("AssignMaterial")]
        public async Task<ActionResult> AssignMaterial([FromBody] AssignMaterialRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request data is required.");
            }

            try
            {
                var task = await _context.Tasks
                    .Include(t => t.TaskMaterialUsages)
                    .ThenInclude(tmu => tmu.Material)
                    .FirstOrDefaultAsync(t => t.ID == request.TaskID);

                if (task == null)
                {
                    return NotFound("Task not found.");
                }

                var material = await _context.Materials.FindAsync(request.MaterialID);
                if (material == null)
                {
                    return NotFound("Material not found.");
                }

                if (!UnitConversionHelper.CanConvert(material.UnitOfIssue, request.UnitOfMeasurement))
                {
                    return BadRequest("Incompatible units of measurement.");
                }

                double convertedAmount = UnitConversionHelper.Convert(request.UnitOfMeasurement, material.UnitOfIssue, request.Amount);

                var taskMaterialUsage = new TaskMaterialUsage
                {
                    TaskID = request.TaskID,
                    MaterialID = request.MaterialID,
                    Amount = (int)convertedAmount,
                    UnitOfMeasurement = request.UnitOfMeasurement,
                    Task = task,
                    Material = material
                };

                _context.TaskMaterialUsages.Add(taskMaterialUsage);
                task.TaskMaterialUsages.Add(taskMaterialUsage);

                _context.Entry(task).State = EntityState.Modified;
                _context.Entry(material).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok("Material assigned to task successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning material: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpGet("{id}/AssignedMaterials")]
        public async Task<ActionResult<IEnumerable<AssignedMaterialDto>>> GetAssignedMaterialsForTask(Guid id)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskMaterialUsages)
                    .ThenInclude(tmu => tmu.Material)
                .FirstOrDefaultAsync(t => t.ID == id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            var assignedMaterials = task.TaskMaterialUsages.Select(tmu => new AssignedMaterialDto
            {
                MaterialPartNumber = tmu.Material.PartNumber,
                Amount = tmu.Amount,
                UnitOfMeasurement = tmu.UnitOfMeasurement
            }).ToList();

            return Ok(assignedMaterials);
        }
        public class AssignedMaterialDto
        {
            public string MaterialPartNumber { get; set; }
            public int Amount { get; set; }
            public Unit UnitOfMeasurement { get; set; }
        }

        [HttpGet("WithMaterials")]
        public async Task<ActionResult<IEnumerable<TaskWithMaterialsDto>>> GetAllTasksWithMaterials()
        {
            var tasks = await _context.Tasks
                .Include(t => t.TaskMaterialUsages)
                    .ThenInclude(tmu => tmu.Material)
                .ToListAsync();

            var taskDtos = tasks.Select(task => new TaskWithMaterialsDto
            {
                TaskID = task.ID,
                TaskName = task.Name,
                TaskDescription = task.Description,
                TotalDuration = task.TotalDuration,
                AssignedMaterials = task.TaskMaterialUsages.Select(tmu => new AssignedMaterialDto
                {
                    MaterialPartNumber = tmu.Material.PartNumber,
                    Amount = tmu.Amount,
                    UnitOfMeasurement = tmu.UnitOfMeasurement
                }).ToList()
            }).ToList();

            return Ok(taskDtos);
        }

        public class AssignMaterialRequest
        {
            public Guid TaskID { get; set; }
            public Guid MaterialID { get; set; }
            public int Amount { get; set; }
            public Unit UnitOfMeasurement { get; set; }
        }
        public class TaskWithMaterialsDto
        {
            public Guid TaskID { get; set; }
            public string TaskName { get; set; }
            public string TaskDescription { get; set; }
            public int TotalDuration { get; set; }
            public List<AssignedMaterialDto> AssignedMaterials { get; set; }
        }

    }
 }