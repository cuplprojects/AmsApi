using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Models
{
    public class AssetCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int? AssetCategoryID { get; set; }
        public string? CategoryName { get; set; }
    }
}
