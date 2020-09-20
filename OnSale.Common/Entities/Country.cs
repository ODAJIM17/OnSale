using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnSale.Common.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "Country")]
        [MaxLength(50, ErrorMessage = "The field {0} can not contain more than {1} characteres.")]
        [Required]
        public string Name { get; set; }

        public ICollection<Department> Departments { get; set; }

        [DisplayName("Dept Count")]
        public int DepartmentsNumber => Departments == null ? 0 : Departments.Count;

    }
}
