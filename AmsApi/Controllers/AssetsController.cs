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
using OfficeOpenXml;
using System.Globalization;

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
                            "status" => query.Where(a => a.Status != null && a.Status == value),
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
                        "assetcategoryid" => isAsc ? query.OrderBy(a => a.AssetCategoryID) : query.OrderByDescending(a => a.AssetCategoryID),
                        "categoryname" => isAsc ? query.OrderBy(a => a.CategoryName) : query.OrderByDescending(a => a.CategoryName),
                        "assetstatusid" => isAsc ? query.OrderBy(a => a.AssetStatusID) : query.OrderByDescending(a => a.AssetStatusID),
                        "status" => isAsc ? query.OrderBy(a => a.Status) : query.OrderByDescending(a => a.Status),
                        "serialnumbermodelnumber" => isAsc ? query.OrderBy(a => a.SerialNumberModelNumber) : query.OrderByDescending(a => a.SerialNumberModelNumber),
                        "modeldetails" => isAsc ? query.OrderBy(a => a.ModelDetails) : query.OrderByDescending(a => a.ModelDetails),
                        "barcodeqrcode" => isAsc ? query.OrderBy(a => a.BarcodeQRCode) : query.OrderByDescending(a => a.BarcodeQRCode),
                        "suppliervendorname" => isAsc ? query.OrderBy(a => a.SupplierVendorName) : query.OrderByDescending(a => a.SupplierVendorName),
                        "invoicenumber" => isAsc ? query.OrderBy(a => a.InvoiceNumber) : query.OrderByDescending(a => a.InvoiceNumber),
                        "assetcondition" => isAsc ? query.OrderBy(a => a.AssetCondition) : query.OrderByDescending(a => a.AssetCondition),
                        "remarksnotes" => isAsc ? query.OrderBy(a => a.RemarksNotes) : query.OrderByDescending(a => a.RemarksNotes),
                        "defaultlocation" => isAsc ? query.OrderBy(a => a.DefaultLocation) : query.OrderByDescending(a => a.DefaultLocation),
                        "assettypeid" => isAsc ? query.OrderBy(a => a.AssetTypeID) : query.OrderByDescending(a => a.AssetTypeID),
                        "assettypename" => isAsc ? query.OrderBy(a => a.AssetTypeName) : query.OrderByDescending(a => a.AssetTypeName),
                        _ => isAsc ? query.OrderBy(a => a.AssetID) : query.OrderByDescending(a => a.AssetID)
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


        [HttpPost("upload-assets-excel")]
        public async Task<IActionResult> UploadAssetsExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Set EPPlus license context (for non-commercial use)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet == null)
                return BadRequest("Invalid Excel file.");

            var assets = new List<Asset>();
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var categoryName = worksheet.Cells[row, 3].Text.Trim();     // CategoryName
                var assetTypeName = worksheet.Cells[row, 4].Text.Trim();    // AssetTypeName
                var statusName = worksheet.Cells[row, 13].Text.Trim();      // Status

                var assetCategory = await _context.assetCategories
                    .FirstOrDefaultAsync(x => x.CategoryName == categoryName);
                var assetType = await _context.AssetTypes
                    .FirstOrDefaultAsync(x => x.AssetTypeName == assetTypeName);
                var assetStatus = await _context.assetStatus
                    .FirstOrDefaultAsync(x => x.Status == statusName);

                // Skip row if any required mapping is missing
                if (assetCategory == null || assetType == null || assetStatus == null)
                    continue;

                var asset = new Asset
                {
                    AssetsName = worksheet.Cells[row, 1].Text,
                    SerialNumberModelNumber = worksheet.Cells[row, 5].Text,
                    ModelDetails = worksheet.Cells[row, 6].Text,
                    BarcodeQRCode = worksheet.Cells[row, 7].Text,
                    PurchaseDate = TryParseDate(worksheet.Cells[row, 8].Text),
                    CostPrice = TryParseInt(worksheet.Cells[row, 9].Text),
                    SupplierVendorName = worksheet.Cells[row, 10].Text,
                    InvoiceNumber = worksheet.Cells[row, 11].Text,
                    WarrantyStartDate = TryParseDate(worksheet.Cells[row, 12].Text),
                    WarrantyEndDate = TryParseDate(worksheet.Cells[row, 13].Text),
                    AssetCondition = worksheet.Cells[row, 14].Text,
                    RemarksNotes = worksheet.Cells[row, 15].Text,
                    DefaultLocation = worksheet.Cells[row, 16].Text,

                    AssetCategoryID = assetCategory.AssetCategoryID,
                    AssetTypeID = assetType.AssetTypeID,
                    AssetStatusID = assetStatus.AssetStatusID
                };

                assets.Add(asset);
            }

            if (assets.Count > 0)
            {
                _context.Assets.AddRange(assets);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                Message = "Assets upload completed",
                UploadedCount = assets.Count,
                SkippedCount = rowCount - 1 - assets.Count // minus header row
            });
        }

        private static DateTime? TryParseDate(string input)
        {
            return DateTime.TryParse(input, out var result) ? result : null;
        }

        private static int? TryParseInt(string input)
        {
            return int.TryParse(input, out var result) ? result : null;
        }



    }
}