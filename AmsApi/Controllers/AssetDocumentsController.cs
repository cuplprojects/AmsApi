using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmsApi.Data;
using AmsApi.Models;
using AmsApi.Services;

namespace AmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetDocumentsController : ControllerBase
    {
        private readonly AMSDbContext _context;
        private readonly ILoggerService _loggerService;

        public AssetDocumentsController(AMSDbContext context, ILoggerService loggerService)
        {
            _context = context;
            _loggerService = loggerService;
        }

        // GET: api/AssetDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetDocument>>> GetAssetDocuments()
        {
            return await _context.AssetDocuments.ToListAsync();
        }

        // GET: api/AssetDocuments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetDocument>> GetAssetDocument(int id)
        {
            var assetDocument = await _context.AssetDocuments.FindAsync(id);

            if (assetDocument == null)
            {
                return NotFound();
            }

            return assetDocument;
        }

        // PUT: api/AssetDocuments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssetDocument(int id, AssetDocument assetDocument)
        {
            if (id != assetDocument.AssetDocumentID)
            {
                return BadRequest();
            }

            _context.Entry(assetDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetDocumentExists(id))
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

        // POST: api/AssetDocuments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostAssetDocument([FromForm] AssetDocumentInput input)
        {
            try
            {
                if (input.File == null || input.File.Length == 0)
                {
                    _loggerService.LogError("File validation failed", "File is missing or empty", "AssetDocumentController");
                    return BadRequest("File is missing.");
                }

                // Validate folder input
                string folderName = input.TargetFolder?.Trim().ToLower() switch
                {
                    "images" => "Images",
                    "documents" => "Documents",
                    _ => "Documents" // default fallback
                };

                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);
                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(input.File.FileName)}";
                string absoluteFilePath = Path.Combine(rootPath, uniqueFileName);
                string relativeFilePath = Path.Combine(folderName, uniqueFileName); // Relative for DB

                // Save file
                using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
                {
                    await input.File.CopyToAsync(stream);
                }

                // Save to DB
                var document = new AssetDocument
                {
                    AssetID = input.AssetID,
                    FilePath = relativeFilePath,
                    FileName = input.File.FileName,
                    Description = input.Description,
                    UploadedAt = DateTime.Now
                };

                _context.AssetDocuments.Add(document);
                await _context.SaveChangesAsync();

                // Log successful document upload event
                _loggerService.LogEvent(
                    message: $"Asset document uploaded successfully for AssetID: {input.AssetID}",
                    category: "Document Upload",
                    triggeredBy: 1,
                    oldValue: null,
                    newValue: $"File: {input.File.FileName}, Path: {relativeFilePath}, Description: {input.Description}"
                );

                return CreatedAtAction("GetAssetDocument", new { id = document.AssetDocumentID }, document);
            }
            catch (Exception ex)
            {
                // Log the error
                _loggerService.LogError(
                    error: "Document upload failed",
                    errormessage: ex.Message,
                    controller: "AssetDocumentController"
                );

                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        public class AssetDocumentInput
        {
            public int AssetID { get; set; }

            public IFormFile File { get; set; }

            public string? Description { get; set; }

            public string? TargetFolder { get; set; } // "Images" or "Documents"
        }




        // DELETE: api/AssetDocuments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetDocument(int id)
        {
            var assetDocument = await _context.AssetDocuments.FindAsync(id);
            if (assetDocument == null)
            {
                return NotFound();
            }

            _context.AssetDocuments.Remove(assetDocument);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetDocumentExists(int id)
        {
            return _context.AssetDocuments.Any(e => e.AssetDocumentID == id);
        }
    }
}
