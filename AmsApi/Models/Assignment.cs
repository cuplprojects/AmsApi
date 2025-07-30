using DocumentFormat.OpenXml.Bibliography;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Models
{
    public class Assignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentID { get; set; }

        // Foreign Key to Asset
      
        public int AssetID { get; set; }
       

        // Foreign Key to Employee
        
        public int? AssignedTo { get; set; }

        public int AssignedById { get; set; }
        

     
        public int? DepartmentID { get; set; }
       

    
        public int? BranchID { get; set; }
        

        [StringLength(255)]
        public string? Location { get; set; }

        public string? AssignmentDocuments { get; set; } // JSON string; for complex data, use a related table or DTO
        public DateTime CreatedDate { get; set; }
        public string? Remarks { get; set; }
    }
}
