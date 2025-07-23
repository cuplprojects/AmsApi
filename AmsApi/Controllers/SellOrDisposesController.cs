using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmsApi.Data;
using AmsApi.Model;

namespace AmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellOrDisposesController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public SellOrDisposesController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/SellOrDisposes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SellOrDispose>>> GetSellOrDisposes()
        {
            return await _context.SellOrDisposes.ToListAsync();
        }

        // GET: api/SellOrDisposes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SellOrDispose>> GetSellOrDispose(int id)
        {
            var sellOrDispose = await _context.SellOrDisposes.FindAsync(id);

            if (sellOrDispose == null)
            {
                return NotFound();
            }

            return sellOrDispose;
        }

        // PUT: api/SellOrDisposes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSellOrDispose(int id, SellOrDispose sellOrDispose)
        {
            if (id != sellOrDispose.SellDisposeID)
            {
                return BadRequest();
            }

            _context.Entry(sellOrDispose).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellOrDisposeExists(id))
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

        // POST: api/SellOrDisposes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SellOrDispose>> PostSellOrDispose(SellOrDispose sellOrDispose)
        {
            _context.SellOrDisposes.Add(sellOrDispose);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSellOrDispose", new { id = sellOrDispose.SellDisposeID }, sellOrDispose);
        }

        // DELETE: api/SellOrDisposes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSellOrDispose(int id)
        {
            var sellOrDispose = await _context.SellOrDisposes.FindAsync(id);
            if (sellOrDispose == null)
            {
                return NotFound();
            }

            _context.SellOrDisposes.Remove(sellOrDispose);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SellOrDisposeExists(int id)
        {
            return _context.SellOrDisposes.Any(e => e.SellDisposeID == id);
        }
    }
}
