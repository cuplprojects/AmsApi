using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmsApi.Model
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(255)]
        public string EmployeeName { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        public int DepartmentID { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }

        public int BranchID { get; set; }
    }
}
