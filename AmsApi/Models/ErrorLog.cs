using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AmsApi.Models
{
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ErrorID { get; set; }

        public string Error { get; set; }

        public string Message { get; set; }

        public string OccuranceSpace { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Add this property
    }
}
