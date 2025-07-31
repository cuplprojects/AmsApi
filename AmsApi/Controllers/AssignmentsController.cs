using AmsApi.Data;
using AmsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;


namespace AmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly AMSDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AssignmentsController(AMSDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
        [HttpPost("upload-assignment")]
        public async Task<IActionResult> UploadAssignment([FromForm] AssignmentUploadRequest input)
        {
            try
            {
                // Ensure AssetID and AssignedTo are provided
                if (input.AssetID == null || input.AssignedTo == null)
                    return BadRequest("AssetID and AssignedTo are required.");

                // Check if the asset is already assigned
                var asset = await _context.Assets.FindAsync(input.AssetID.Value);
                if (asset == null)
                    return NotFound("Asset not found.");

                if (asset.AssetStatusID == 1)
                    return BadRequest("Asset is already assigned.");

                var savedFilePaths = new List<string>();
                var wwwRootPath = _env.WebRootPath;
                var folderPath = Path.Combine(wwwRootPath, "AssignmentDocuments");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Only process files if provided
                if (input.Files != null)
                {
                    foreach (var file in input.Files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(folderPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            savedFilePaths.Add("/AssignmentDocuments/" + fileName); // Use relative path
                        }
                    }
                }

                var assignment = new Assignment
                {
                    AssetID = input.AssetID.Value,
                    AssignedTo = input.AssignedTo.Value,
                    AssignedById = input.AssignedById ?? 0,
                    DepartmentID = input.DepartmentID ?? 0,
                    Company = string.IsNullOrWhiteSpace(input.Company) ? null : input.Company.Trim(),
                    Remarks = string.IsNullOrWhiteSpace(input.Remarks) ? null : input.Remarks.Trim(),
                    AssignmentDocuments = JsonSerializer.Serialize(savedFilePaths),
                    CreatedDate = DateTime.Now
                };

                _context.Assignments.Add(assignment);

                // Mark asset as assigned
                asset.AssetStatusID = 1;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Assignment uploaded and asset status updated successfully." });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"DB Update Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"General Error: {ex.Message}");
            }
        }







        public class AssignmentUploadRequest
        {
            public int? AssetID { get; set; }
            public int? AssignedTo { get; set; }
            public int? AssignedById { get; set; }
            public int? DepartmentID { get; set; }
            public string? Company { get; set; }
            public string? Location { get; set; }
            public string? Remarks { get; set; }
            public List<IFormFile>? Files { get; set; }
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
