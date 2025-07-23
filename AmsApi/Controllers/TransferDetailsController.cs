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
    public class TransferDetailsController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public TransferDetailsController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/TransferDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferDetails>>> GetTransferDetails()
        {
            return await _context.TransferDetails.ToListAsync();
        }

        // GET: api/TransferDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransferDetails>> GetTransferDetails(int id)
        {
            var transferDetails = await _context.TransferDetails.FindAsync(id);

            if (transferDetails == null)
            {
                return NotFound();
            }

            return transferDetails;
        }

        // PUT: api/TransferDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransferDetails(int id, TransferDetails transferDetails)
        {
            if (id != transferDetails.TransferID)
            {
                return BadRequest();
            }

            _context.Entry(transferDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransferDetailsExists(id))
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

        // POST: api/TransferDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TransferDetails>> PostTransferDetails(TransferDetails transferDetails)
        {
            _context.TransferDetails.Add(transferDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransferDetails", new { id = transferDetails.TransferID }, transferDetails);
        }

        // DELETE: api/TransferDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransferDetails(int id)
        {
            var transferDetails = await _context.TransferDetails.FindAsync(id);
            if (transferDetails == null)
            {
                return NotFound();
            }

            _context.TransferDetails.Remove(transferDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransferDetailsExists(int id)
        {
            return _context.TransferDetails.Any(e => e.TransferID == id);
        }
    }
}
