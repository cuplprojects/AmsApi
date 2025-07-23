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
    public class AssetTypesController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public AssetTypesController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/AssetTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetType>>> GetAssetTypes()
        {
            return await _context.AssetTypes.ToListAsync();
        }

        // GET: api/AssetTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetType>> GetAssetType(int id)
        {
            var assetType = await _context.AssetTypes.FindAsync(id);

            if (assetType == null)
            {
                return NotFound();
            }

            return assetType;
        }

        // PUT: api/AssetTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssetType(int id, AssetType assetType)
        {
            if (id != assetType.AssetTypeID)
            {
                return BadRequest();
            }

            _context.Entry(assetType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetTypeExists(id))
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

        // POST: api/AssetTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssetType>> PostAssetType(AssetType assetType)
        {
            _context.AssetTypes.Add(assetType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssetType", new { id = assetType.AssetTypeID }, assetType);
        }

        // DELETE: api/AssetTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetType(int id)
        {
            var assetType = await _context.AssetTypes.FindAsync(id);
            if (assetType == null)
            {
                return NotFound();
            }

            _context.AssetTypes.Remove(assetType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetTypeExists(int id)
        {
            return _context.AssetTypes.Any(e => e.AssetTypeID == id);
        }
    }
}
