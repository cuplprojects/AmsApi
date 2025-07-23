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
    public class AssetRequestsController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public AssetRequestsController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/AssetRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetRequest>>> GetAssetRequests()
        {
            return await _context.AssetRequests.ToListAsync();
        }

        // GET: api/AssetRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetRequest>> GetAssetRequest(int id)
        {
            var assetRequest = await _context.AssetRequests.FindAsync(id);

            if (assetRequest == null)
            {
                return NotFound();
            }

            return assetRequest;
        }

        // PUT: api/AssetRequests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssetRequest(int id, AssetRequest assetRequest)
        {
            if (id != assetRequest.RequestID)
            {
                return BadRequest();
            }

            _context.Entry(assetRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetRequestExists(id))
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

        // POST: api/AssetRequests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AssetRequest>> PostAssetRequest(AssetRequest assetRequest)
        {
            _context.AssetRequests.Add(assetRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssetRequest", new { id = assetRequest.RequestID }, assetRequest);
        }

        // DELETE: api/AssetRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssetRequest(int id)
        {
            var assetRequest = await _context.AssetRequests.FindAsync(id);
            if (assetRequest == null)
            {
                return NotFound();
            }

            _context.AssetRequests.Remove(assetRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetRequestExists(int id)
        {
            return _context.AssetRequests.Any(e => e.RequestID == id);
        }
    }
}
