using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSoftware.Data;
using TaskManagementSoftware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagementSoftware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Materials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetMaterials()
        {
            return await _context.Materials.ToListAsync();
        }

        // GET: api/Materials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Material>> GetMaterial(Guid id)
        {
            var material = await _context.Materials.FindAsync(id);

            if (material == null)
            {
                return NotFound();
            }

            return material;
        }

        // POST: api/Materials
        [HttpPost]
        public async Task<ActionResult<Material>> PostMaterial([FromBody] MaterialDto materialDto)
        {
            if (materialDto == null)
            {
                return BadRequest("Material data is required.");
            }

           
            if (!Enum.TryParse(materialDto.UnitOfIssue, true, out Unit unitOfIssueEnum))
            {
                return BadRequest("Invalid unitOfIssue value.");
            }

            var material = new Material
            {
                ID = Guid.NewGuid(),
                PartNumber = materialDto.PartNumber,
                ManufacturerCode = materialDto.ManufacturerCode,
                Price = materialDto.Price,
                UnitOfIssue = unitOfIssueEnum
            };

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMaterial), new { id = material.ID }, material);
        }

        // PUT: api/Materials/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterial(Guid id, [FromBody] Material material)
        {
            if (id != material.ID)
            {
                return BadRequest("Material ID mismatch.");
            }

            _context.Entry(material).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialExists(id))
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

        // DELETE: api/Materials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(Guid id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            return NoContent();
        }

      
        private bool MaterialExists(Guid id)
        {
            return _context.Materials.Any(e => e.ID == id);
        }
    }
}
