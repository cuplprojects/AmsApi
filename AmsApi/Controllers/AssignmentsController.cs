using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmsApi.Data;
using AmsApi.Models;

namespace AmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public AssignmentsController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/Assignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignments()
        {
            return await _context.Assignments.ToListAsync();
        }

        // GET: api/Assignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
            {
                return NotFound();
            }

            return assignment;
        }

        // PUT: api/Assignments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignment(int id, Assignment assignment)
        {
            if (id != assignment.AssignmentID)
            {
                return BadRequest();
            }

            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id))
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

        // POST: api/Assignments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostAssignment([FromForm] AssignmentInput input)
        {
            try
            {
                if (input.Files == null || input.Files.Count == 0)
                {
                    return BadRequest("At least one file is required.");
                }

                string folderName = "AssignmentDoc";
                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                List<string> filePaths = new();

                foreach (var file in input.Files)
                {
                    if (file.Length > 0)
                    {
                        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        string absolutePath = Path.Combine(rootPath, uniqueFileName);
                        string relativePath = Path.Combine(folderName, uniqueFileName); // For DB

                        using (var stream = new FileStream(absolutePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        filePaths.Add(relativePath);
                    }
                }

                var assignment = new Assignment
                {
                    AssetID = input.AssetID,
                    AssignedTo = input.AssignedTo,
                    AssignedById = input.AssignedById,
                    DepartmentID = input.DepartmentID,
                    BranchID = input.BranchID,
                    Location = input.Location,
                    Remarks = input.Remarks,
                    CreatedDate = DateTime.Now,
                    AssignmentDocuments = System.Text.Json.JsonSerializer.Serialize(filePaths)
                };

                _context.Assignments.Add(assignment);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAssignment", new { id = assignment.AssignmentID }, assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }



        public class AssignmentInput
        {
            public int AssetID { get; set; }
            public int? AssignedTo { get; set; }
            public int AssignedById { get; set; }
            public int? DepartmentID { get; set; }
            public int? BranchID { get; set; }
            public string? Location { get; set; }
            public string? Remarks { get; set; }

            [FromForm]
            public List<IFormFile> Files { get; set; } = new();
        }


        // DELETE: api/Assignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.AssignmentID == id);
        }
    }
}
