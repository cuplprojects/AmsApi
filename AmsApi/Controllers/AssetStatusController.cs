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
    public class AssetStatusController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public AssetStatusController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/AssetStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetStatus>>> GetassetStatus()
        {
            return await _context.assetStatus.ToListAsync();
        }

        // GET: api/AssetStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetStatus>> GetAssetStatus(int? id)
        {
            var assetStatus = await _context.assetStatus.FindAsync(id);

            if (assetStatus == null)
            {
                return NotFound();
            }

            return assetStatus;
        }

        // PUT: api/AssetStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssetStatus(int? id, AssetStatus assetStatus)
        {
            if (id != assetStatus.AssetStatusID)
            {
                return BadRequest();
            }

            _context.Entry(assetStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetStatusExists(id))
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

        // POST: api/AssetStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssetStatus>> PostAssetStatus(AssetStatus assetStatus)
        {
            _context.assetStatus.Add(assetStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssetStatus", new { id = assetStatus.AssetStatusID }, assetStatus);
        }

        // DELETE: api/AssetStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetStatus(int? id)
        {
            var assetStatus = await _context.assetStatus.FindAsync(id);
            if (assetStatus == null)
            {
                return NotFound();
            }

            _context.assetStatus.Remove(assetStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetStatusExists(int? id)
        {
            return _context.assetStatus.Any(e => e.AssetStatusID == id);
        }
    }
}
