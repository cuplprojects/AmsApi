
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Models
{
    public class AssetsDep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetsDepID { get; set; }
        public int AssetID { get; set; }
        public int AssetCategoryID { get; set; }
        public string Year { get; set; }
        public double AccumulatedDep {  get; set; }
        public double Depreciation {  get; set; }
        public double YearAsOn31 { get; set; }


    }
}
