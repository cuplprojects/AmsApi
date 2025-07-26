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
            try
            {
                // Join AssetCategory and AssetStatus tables
                var query = from a in _context.Assets
                            join ac in _context.assetCategories on a.AssetCategoryID equals ac.AssetCategoryID into acGroup
                            from ac in acGroup.DefaultIfEmpty()
                            join ast in _context.assetStatus on a.AssetStatusID equals ast.AssetStatusID into astGroup
                            from ast in astGroup.DefaultIfEmpty()
                            select new
                            {
                                Asset = a,
                                CategoryName = ac != null ? ac.CategoryName : null,
                                Status = ast != null ? ast.Status : null
                            };

                // Search logic
                if (!string.IsNullOrWhiteSpace(search))
                {
                    string lowerSearch = search.ToLower();

                    if (!string.IsNullOrWhiteSpace(searchColumn))
                    {
                        switch (searchColumn.ToLower())
                        {
                            case "assetsname":
                                query = query.Where(q => q.Asset.AssetsName.ToLower().Contains(lowerSearch));
                                break;
                            case "assettype":
                                query = query.Where(q => q.Asset.AssetTypeID.ToString().ToLower().Contains(lowerSearch));
                                break;
                            case "categoryname":
                                query = query.Where(q => (q.CategoryName ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "serialnumbermodelnumber":
                                query = query.Where(q => (q.Asset.SerialNumberModelNumber ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "modeldetails":
                                query = query.Where(q => (q.Asset.ModelDetails ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "barcodeqrcode":
                                query = query.Where(q => (q.Asset.BarcodeQRCode ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "suppliervendorname":
                                query = query.Where(q => (q.Asset.SupplierVendorName ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "invoicenumber":
                                query = query.Where(q => (q.Asset.InvoiceNumber ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "status":
                                query = query.Where(q => (q.Status ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "assetcondition":
                                query = query.Where(q => (q.Asset.AssetCondition ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "remarksnotes":
                                query = query.Where(q => (q.Asset.RemarksNotes ?? "").ToLower().Contains(lowerSearch));
                                break;
                            case "defaultlocation":
                                query = query.Where(q => (q.Asset.DefaultLocation ?? "").ToLower().Contains(lowerSearch));
                                break;
                            default:
                                _loggerService.LogError(
                                    error: "Invalid search column provided",
                                    errormessage: $"SearchColumn '{searchColumn}' is not valid",
                                    controller: "AssetController"
                                );
                                return BadRequest("Invalid searchColumn.");
                        }
                    }
                    else
                    {
                        // Global search (all fields)
                        query = query.Where(q =>
                            q.Asset.AssetsName.ToLower().Contains(lowerSearch) ||
                            q.Asset.AssetTypeID.ToString().ToLower().Contains(lowerSearch) ||
                            (q.CategoryName ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.SerialNumberModelNumber ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.ModelDetails ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.BarcodeQRCode ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.SupplierVendorName ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.InvoiceNumber ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Status ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.AssetCondition ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.RemarksNotes ?? "").ToLower().Contains(lowerSearch) ||
                            (q.Asset.DefaultLocation ?? "").ToLower().Contains(lowerSearch)
                        );
                    }
                }

                // Validate pagination
                if (page < 1)
                {
                    _loggerService.LogError("Invalid pagination parameter", $"Page number must be greater than 0. Provided: {page}", "AssetController");
                    return BadRequest("Page number must be greater than 0.");
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    _loggerService.LogError("Invalid page size parameter", $"Page size must be between 1 and 100. Provided: {pageSize}", "AssetController");
                    return BadRequest("Page size must be between 1 and 100.");
                }

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Sorting logic with if-else to avoid switch expression error
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    try
                    {
                        if (sortOrder?.ToLower() == "desc")
                        {
                            if (sortBy.ToLower() == "categoryname")
                                query = query.OrderByDescending(q => q.CategoryName);
                            else if (sortBy.ToLower() == "status")
                                query = query.OrderByDescending(q => q.Status);
                            else
                                query = query.OrderByDescending(q => EF.Property<object>(q.Asset, sortBy));
                        }
                        else
                        {
                            if (sortBy.ToLower() == "categoryname")
                                query = query.OrderBy(q => q.CategoryName);
                            else if (sortBy.ToLower() == "status")
                                query = query.OrderBy(q => q.Status);
                            else
                                query = query.OrderBy(q => EF.Property<object>(q.Asset, sortBy));
                        }
                    }
                    catch (Exception sortEx)
                    {
                        _loggerService.LogError("Invalid sort column", $"SortBy column '{sortBy}' is not valid. Error: {sortEx.Message}", "AssetController");
                        return BadRequest("Invalid sortBy column.");
                    }
                }

                // Final paginated result
                var pagedData = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(q => new
                    {
                        q.Asset.AssetID,
                        q.Asset.AssetsName,
                        q.Asset.AssetCategoryID,
                        CategoryName = q.CategoryName,
                        q.Asset.AssetStatusID,
                        Status = q.Status,
                        q.Asset.SerialNumberModelNumber,
                        q.Asset.ModelDetails,
                        q.Asset.BarcodeQRCode,
                        q.Asset.SupplierVendorName,
                        q.Asset.InvoiceNumber,
                        q.Asset.AssetCondition,
                        q.Asset.RemarksNotes,
                        q.Asset.DefaultLocation,
                        q.Asset.AssetTypeID
                    })
                    .ToListAsync();

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
            catch (Exception ex)
            {
                _loggerService.LogError("Failed to retrieve assets", ex.Message, "AssetController");
                return StatusCode(500, "Internal server error occurred while retrieving assets.");
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

            // Fetch all related documents
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

            var response = new
            {
                assetInfo = asset,
                document = documents,            // returns List not single item
                assignmentDetails = new object[] { },  // empty array
                history = new object[] { },            // empty array
                maintenance = new object[] { }         // empty array
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