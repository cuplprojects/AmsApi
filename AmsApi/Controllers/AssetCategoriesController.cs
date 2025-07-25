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
    public class AssetCategoriesController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public AssetCategoriesController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/AssetCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetCategory>>> GetassetCategories()
        {
            return await _context.assetCategories.ToListAsync();
        }

        // GET: api/AssetCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetCategory>> GetAssetCategory(int? id)
        {
            var assetCategory = await _context.assetCategories.FindAsync(id);

            if (assetCategory == null)
            {
                return NotFound();
            }

            return assetCategory;
        }

        // PUT: api/AssetCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssetCategory(int? id, AssetCategory assetCategory)
        {
            if (id != assetCategory.AssetCategoryID)
            {
                return BadRequest();
            }

            _context.Entry(assetCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetCategoryExists(id))
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

        // POST: api/AssetCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssetCategory>> PostAssetCategory(AssetCategory assetCategory)
        {
            _context.assetCategories.Add(assetCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssetCategory", new { id = assetCategory.AssetCategoryID }, assetCategory);
        }

        // DELETE: api/AssetCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetCategory(int? id)
        {
            var assetCategory = await _context.assetCategories.FindAsync(id);
            if (assetCategory == null)
            {
                return NotFound();
            }

            _context.assetCategories.Remove(assetCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetCategoryExists(int? id)
        {
            return _context.assetCategories.Any(e => e.AssetCategoryID == id);
        }
    }
}
