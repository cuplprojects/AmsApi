using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AmsApi.Models
{
    public class AssetStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int? AssetStatusID { get; set; }

        public string? Status { get; set; }
    }
}
