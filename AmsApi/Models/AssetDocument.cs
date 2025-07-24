using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Models
{
    public class AssetDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetDocumentID { get; set; }

        [Required]
        public int AssetID { get; set; }


        [Required]
        [StringLength(1000)]
        public string FilePath { get; set; }  // Path to the stored document/image


        [StringLength(255)]
        public string? FileName { get; set; }  // Optional: original file name

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Description { get; set; }  // Optional description
    }
}
