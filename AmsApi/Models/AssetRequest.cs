using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Models
{
    public class AssetRequest
    {
        [Key]
        public int RequestID { get; set; }

      
        public int Requested { get; set; }


    
        public int AssetTypeID { get; set; }
      

        [StringLength(500)]
        public string? Reason { get; set; }

        public DateTime? RequestDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }  // Pending / Approved / Rejected

        // Foreign key to approver employee
        [ForeignKey("ApprovedByEmployee")]
        public int? ApprovedBy { get; set; }
       

        public string? RequestDocuments { get; set; } // JSON (use string or JsonDocument)
    }
}
