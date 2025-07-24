using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace AmsApi.Models
{
    public class Asset
    {
        [Key]
        public int AssetID { get; set; }

        [Required]
        [StringLength(255)]
        public string AssetsName { get; set; }

      
        //public int AssetTypeID { get; set; }
        public int AssetType { get; set; }

       
        public int? AssetCategoryID { get; set; }

        [StringLength(255)]
        public string? SerialNumberModelNumber { get; set; }

        [StringLength(1000)]
        public string? ModelDetails { get; set; }

        [StringLength(100)]
        public string? BarcodeQRCode { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int? CostPrice { get; set; }

        [StringLength(255)]
        public string? SupplierVendorName { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        public DateTime? WarrantyStartDate { get; set; }

        public DateTime? WarrantyEndDate { get; set; }

        //public string? UploadedDocuments { get; set; } // You can also use JsonDocument or a DTO for complex structures

        [StringLength(500)]
        public string? AMCDetails { get; set; }

        [StringLength(20)]
        public string? CurrentStatus { get; set; } // Assigned / Unassigned / Under Repair / Retired

        [StringLength(20)]
        public string? AssetCondition { get; set; } // Good / Average / Needs Repair / Retired

        [StringLength(1000)]
        public string? RemarksNotes { get; set; }

        public string? DefaultLocation { get; set; } // This can be a JSON string or a simple string, depending on your needs
        
    }
}
