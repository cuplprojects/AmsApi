using System;
using System.Collections.Generic;
using System.IO;
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
    public class AssetDocumentsController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public AssetDocumentsController(AMSDbContext context)
        {
            _context = context;
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

        // Standard POST (without file upload)
        [HttpPost]
        public async Task<ActionResult<AssetDocument>> PostAssetDocument(AssetDocument assetDocument)
        {
            _context.AssetDocuments.Add(assetDocument);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssetDocument", new { id = assetDocument.AssetDocumentID }, assetDocument);
        }

        // POST: api/AssetDocuments/upload
        [HttpPost("upload")]
        public async Task<ActionResult<AssetDocument>> UploadAssetDocument(
            [FromForm] int assetId,
            [FromForm] IFormFile file,
            [FromForm] string? description)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Decide folder
            string folder = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ? "Images" : "Documents";
            string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

            if (!Directory.Exists(wwwRootPath))
                Directory.CreateDirectory(wwwRootPath);

            string originalFileName = Path.GetFileName(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
            string savedFilePath = Path.Combine(wwwRootPath, uniqueFileName);

            // Save file to disk
            using (var fileStream = new FileStream(savedFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Create relative path for the database
            string relativeFilePath = Path.Combine(folder, uniqueFileName).Replace("\\", "/");

            var assetDocument = new AssetDocument
            {
                AssetID = assetId,
                FilePath = relativeFilePath,
                FileType = file.ContentType,
                FileName = originalFileName,
                UploadedAt = DateTime.Now,
                Description = description
            };

            _context.AssetDocuments.Add(assetDocument);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssetDocument), new { id = assetDocument.AssetDocumentID }, assetDocument);
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

            // Delete file from disk
            if (!string.IsNullOrEmpty(assetDocument.FilePath))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", assetDocument.FilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
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
