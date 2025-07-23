using System.ComponentModel.DataAnnotations;

namespace AmsApi.Models
{
    public class AssetType
    {
        [Key]
        public int AssetTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string AssetTypeName { get; set; }
    }
}
