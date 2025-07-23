using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Models
{
    public class TransferDetails
    {
        [Key]
        public int TransferID { get; set; }

        // Foreign key to Asset
        [ForeignKey("Asset")]
        public int AssetID { get; set; }
        public Asset Asset { get; set; }

        [StringLength(20)]
        public string? TransferType { get; set; } // Company / Branch


        public int TransferredFromBranchID { get; set; }
 

        public int TransferredToBranchID { get; set; }


        public DateTime? TransferDate { get; set; }


        public int? ApprovedBy { get; set; }


        [StringLength(50)]
        public string? TransferStatus { get; set; } // Pending / Approved / Completed

        public string? TransferDocuments { get; set; } // JSON (could be string or JsonDocument)
    }
}
