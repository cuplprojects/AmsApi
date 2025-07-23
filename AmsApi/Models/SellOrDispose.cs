using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Model
{
    public class SellOrDispose
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SellDisposeID { get; set; }

        [Required]
        public int AssetID { get; set; }


        [Required]
        [StringLength(20)]
        public string SellType { get; set; }  // e.g., "Sold", "Scrapped", "Donated"

        [Column(TypeName = "decimal(12,2)")]
        public decimal Value { get; set; }

        [StringLength(255)]
        public string? BuyerRecipientName { get; set; }

        public DateTime TransactionDate { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }
    }
}
