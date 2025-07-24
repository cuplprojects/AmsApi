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
    public class AssetsController : ControllerBase
    {
        private readonly AMSDbContext _context;

        public AssetsController(AMSDbContext context)
        {
            _context = context;
        }

        // GET: api/Assets
        [HttpGet]
        public async Task<ActionResult<object>> GetAssets(
           [FromQuery] string? search,
           [FromQuery] string? searchColumn,
           [FromQuery] string? sortBy = "AssetsName",
           [FromQuery] string? sortOrder = "asc",
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 10
       )
        {
            IQueryable<Asset> query = _context.Assets;

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();

                if (!string.IsNullOrWhiteSpace(searchColumn))
                {
                    switch (searchColumn.ToLower())
                    {
                        case "assetsname":
                            query = query.Where(a => a.AssetsName.ToLower().Contains(lowerSearch));
                            break;
                        case "assettype":
                            query = query.Where(a => a.AssetType.ToString().ToLower().Contains(lowerSearch));
                            break;
                        case "assetcategoryid":
                            query = query.Where(a => a.AssetCategoryID.ToString().Contains(lowerSearch));
                            break;
                        case "serialnumbermodelnumber":
                            query = query.Where(a => (a.SerialNumberModelNumber ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "modeldetails":
                            query = query.Where(a => (a.ModelDetails ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "barcodeqrcode":
                            query = query.Where(a => (a.BarcodeQRCode ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "suppliervendorname":
                            query = query.Where(a => (a.SupplierVendorName ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "invoicenumber":
                            query = query.Where(a => (a.InvoiceNumber ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "amcdetails":
                            query = query.Where(a => (a.AMCDetails ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "currentstatus":
                            query = query.Where(a => (a.CurrentStatus ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "assetcondition":
                            query = query.Where(a => (a.AssetCondition ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "remarksnotes":
                            query = query.Where(a => (a.RemarksNotes ?? "").ToLower().Contains(lowerSearch));
                            break;
                        case "defaultlocation":
                            query = query.Where(a => (a.DefaultLocation ?? "").ToLower().Contains(lowerSearch));
                            break;
                        default:
                            // Invalid search column, optionally return BadRequest
                            return BadRequest("Invalid searchColumn.");
                    }
                }
                else
                {
                    // Search across all fields
                    query = query.Where(a =>
                        a.AssetsName.ToLower().Contains(lowerSearch) ||
                        a.AssetType.ToString().ToLower().Contains(lowerSearch) ||
                        a.AssetCategoryID.ToString().Contains(lowerSearch) ||
                        (a.SerialNumberModelNumber ?? "").ToLower().Contains(lowerSearch) ||
                        (a.ModelDetails ?? "").ToLower().Contains(lowerSearch) ||
                        (a.BarcodeQRCode ?? "").ToLower().Contains(lowerSearch) ||
                        (a.SupplierVendorName ?? "").ToLower().Contains(lowerSearch) ||
                        (a.InvoiceNumber ?? "").ToLower().Contains(lowerSearch) ||
                        (a.AMCDetails ?? "").ToLower().Contains(lowerSearch) ||
                        (a.CurrentStatus ?? "").ToLower().Contains(lowerSearch) ||
                        (a.AssetCondition ?? "").ToLower().Contains(lowerSearch) ||
                        (a.RemarksNotes ?? "").ToLower().Contains(lowerSearch) ||
                        (a.DefaultLocation ?? "").ToLower().Contains(lowerSearch)
                    );
                }
            }

            // Count total
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Sort dynamically using EF.Property (database-side)
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                try
                {
                    query = sortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                        : query.OrderBy(e => EF.Property<object>(e, sortBy));
                }
                catch
                {
                    return BadRequest("Invalid sortBy column.");
                }
            }

            // Apply pagination
            var pagedData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Return response
            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize,
                SearchColumn = searchColumn,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Data = pagedData
            });
        }






        // GET: api/Assets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> GetAsset(int id)
        {
            var asset = await _context.Assets.FindAsync(id);

            if (asset == null)
            {
                return NotFound();
            }

            return asset;
        }

        // PUT: api/Assets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsset(int id, Asset asset)
        {
            if (id != asset.AssetID)
            {
                return BadRequest();
            }

            _context.Entry(asset).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetExists(id))
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

        // POST: api/Assets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Asset>> PostAsset(Asset asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAsset", new { id = asset.AssetID }, asset);
        }

        // DELETE: api/Assets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.AssetID == id);
        }
    }
}
