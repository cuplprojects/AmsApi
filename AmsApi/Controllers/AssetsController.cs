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
    public class AssetsController : ControllerBase
    {
        private readonly AMSDbContext _context;
        private readonly ILoggerService _loggerService;

        public AssetsController(AMSDbContext context, ILoggerService loggerService)
        {
            _context = context;
            _loggerService = loggerService;
        }


        [HttpGet("GetAssets")]
        public async Task<ActionResult<object>> GetAssets(
         [FromQuery] string? sortBy = "AssetsName",
         [FromQuery] string? sortOrder = "asc",
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 10,
         [FromQuery] string? search = null,
         [FromQuery] Dictionary<string, string>? filters = null)
        {
            try
            {
                var query = from asset in _context.Assets
                            join type in _context.AssetTypes on asset.AssetTypeID equals type.AssetTypeID into at
                            from assetType in at.DefaultIfEmpty()
                            join category in _context.assetCategories on asset.AssetCategoryID equals category.AssetCategoryID into ac
                            from assetCategory in ac.DefaultIfEmpty()
                            join status in _context.assetStatus on asset.AssetStatusID equals status.AssetStatusID into ast
                            from assetStatus in ast.DefaultIfEmpty()
                            select new
                            {
                                asset.AssetID,
                                asset.AssetsName,
                                asset.AssetCategoryID,
                                CategoryName = assetCategory != null ? assetCategory.CategoryName : null,
                                asset.AssetStatusID,
                                Status = assetStatus != null ? assetStatus.Status : null,
                                asset.SerialNumberModelNumber,
                                asset.ModelDetails,
                                asset.BarcodeQRCode,
                                asset.SupplierVendorName,
                                asset.InvoiceNumber,
                                asset.AssetCondition,
                                asset.RemarksNotes,
                                asset.DefaultLocation,
                                asset.AssetTypeID,
                                AssetTypeName = assetType != null ? assetType.AssetTypeName : null
                            };

                // Global Search
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var lowerSearch = search.ToLower();
                    query = query.Where(a =>
                        (a.AssetsName != null && a.AssetsName.ToLower().Contains(lowerSearch)) ||
                        (a.CategoryName != null && a.CategoryName.ToLower().Contains(lowerSearch)) ||
                        (a.ModelDetails != null && a.ModelDetails.ToLower().Contains(lowerSearch)) ||
                        (a.BarcodeQRCode != null && a.BarcodeQRCode.ToLower().Contains(lowerSearch)) ||
                        (a.SupplierVendorName != null && a.SupplierVendorName.ToLower().Contains(lowerSearch)) ||
                        (a.InvoiceNumber != null && a.InvoiceNumber.ToLower().Contains(lowerSearch)) ||
                        (a.AssetCondition != null && a.AssetCondition.ToLower().Contains(lowerSearch)) ||
                        (a.RemarksNotes != null && a.RemarksNotes.ToLower().Contains(lowerSearch)) ||
                        (a.DefaultLocation != null && a.DefaultLocation.ToLower().Contains(lowerSearch)) ||
                        (a.AssetTypeName != null && a.AssetTypeName.ToLower().Contains(lowerSearch)) ||
                        (a.Status != null && a.Status.ToLower().Contains(lowerSearch)) ||
                        (a.SerialNumberModelNumber != null && a.SerialNumberModelNumber.ToLower().Contains(lowerSearch))
                    );
                }

                // Filtering
                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        var key = filter.Key.ToLower();
                        var value = filter.Value.ToLower();

                        query = key switch
                        {
                            "categoryname" => query.Where(a => a.CategoryName != null && a.CategoryName.ToLower().Contains(value)),
                            "modeldetails" => query.Where(a => a.ModelDetails != null && a.ModelDetails.ToLower().Contains(value)),
                            "barcodeqrcode" => query.Where(a => a.BarcodeQRCode != null && a.BarcodeQRCode.ToLower().Contains(value)),
                            "assetsname" => query.Where(a => a.AssetsName != null && a.AssetsName.ToLower().Contains(value)),
                            "assettypename" => query.Where(a => a.AssetTypeName != null && a.AssetTypeName.ToLower().Contains(value)),
                            "serialnumbermodelnumber" => query.Where(a => a.SerialNumberModelNumber != null && a.SerialNumberModelNumber.ToLower().Contains(value)),
                            "assetcondition" => query.Where(a => a.AssetCondition != null && a.AssetCondition.ToLower().Contains(value)),
                            "defaultlocation" => query.Where(a => a.DefaultLocation != null && a.DefaultLocation.ToLower().Contains(value)),
                            "suppliervendorname" => query.Where(a => a.SupplierVendorName != null && a.SupplierVendorName.ToLower().Contains(value)),
                            "invoicenumber" => query.Where(a => a.InvoiceNumber != null && a.InvoiceNumber.ToLower().Contains(value)),
                            "remarksnotes" => query.Where(a => a.RemarksNotes != null && a.RemarksNotes.ToLower().Contains(value)),
                            "status" => query.Where(a => a.Status != null && a.Status.ToLower().Contains(value)),
                            _ => query
                        };
                    }
                }

                // Sorting
                if (!string.IsNullOrEmpty(sortBy))
                {
                    var isAsc = sortOrder?.ToLower() != "desc";
                    query = sortBy.ToLower() switch
                    {
                        "assetid" => isAsc ? query.OrderBy(a => a.AssetID) : query.OrderByDescending(a => a.AssetID),
                        "assetsname" => isAsc ? query.OrderBy(a => a.AssetsName) : query.OrderByDescending(a => a.AssetsName),
                        "categoryname" => isAsc ? query.OrderBy(a => a.CategoryName) : query.OrderByDescending(a => a.CategoryName),
                        "assettypename" => isAsc ? query.OrderBy(a => a.AssetTypeName) : query.OrderByDescending(a => a.AssetTypeName),
                        "modeldetails" => isAsc ? query.OrderBy(a => a.ModelDetails) : query.OrderByDescending(a => a.ModelDetails),
                        _ => query.OrderBy(a => a.AssetID)
                    };
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var result = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    totalCount,
                    totalPages,
                    page,
                    pageSize,
                    search,
                    filters,
                    sortBy = sortBy?.ToLower(),
                    sortOrder = sortOrder?.ToLower(),
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error occurred while retrieving assets: {ex.Message}");
            }
        }







        // GET: api/Assets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAsset(int id)
        {
            var asset = await _context.Assets.FindAsync(id);

            if (asset == null)
            {
                return NotFound();
            }

            // Get CategoryName
            var categoryName = await _context.assetCategories
                .Where(c => c.AssetCategoryID == asset.AssetCategoryID)
                .Select(c => c.CategoryName)
                .FirstOrDefaultAsync();

            // Get AssetTypeName
            var assetTypeName = await _context.AssetTypes
                .Where(t => t.AssetTypeID == asset.AssetTypeID)
                .Select(t => t.AssetTypeName)
                .FirstOrDefaultAsync();

            // Get Status (from AssetStatus)
            var status = await _context.assetStatus
                .Where(s => s.AssetStatusID == asset.AssetStatusID)
                .Select(s => s.Status)
                .FirstOrDefaultAsync();

            // Get related documents
            var documents = await _context.AssetDocuments
                .Where(d => d.AssetID == id)
                .Select(d => new
                {
                    d.AssetDocumentID,
                    d.Description,
                    d.FileName,
                    d.FilePath,
                    d.UploadedAt
                })
                .ToListAsync();

            // Flattened asset info with category, type, and status
            var assetInfo = new
            {
                asset.AssetID,
                asset.AssetsName,
                asset.AssetCategoryID,
                CategoryName = categoryName,
                asset.AssetTypeID,
                AssetTypeName = assetTypeName,
                asset.AssetStatusID,
                Status = status,
                asset.SerialNumberModelNumber,
                asset.ModelDetails,
                asset.BarcodeQRCode,
                asset.PurchaseDate,
                asset.CostPrice,
                asset.SupplierVendorName,
                asset.InvoiceNumber,
                asset.WarrantyStartDate,
                asset.WarrantyEndDate,
                asset.AssetCondition,
                asset.RemarksNotes,
                asset.DefaultLocation
            };

            var response = new
            {
                assetInfo = assetInfo,
                document = documents,
                assignmentDetails = new object[] { },
                history = new object[] { },
                maintenance = new object[] { }
            };

            return Ok(response);
        }






        public class AssetDetailsDto
        {
            public object AssetInfo { get; set; }
            public object Document { get; set; }
            public object AssignmentDetails { get; set; } = new { };
            public object History { get; set; } = new { };
            public object Maintance { get; set; } = new { };
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
