using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Notebooks.Models
{
    public class Notebook
    {
        [Key]
        public int Id { get; set; }
        [StringLength(47, MinimumLength = 1)]
        [Required]
        public string Name { get; set; }
        [StringLength(47, MinimumLength = 1)]
        [Required]
        public string Surname { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Range(0, 110)]
        [Required]            
        public int Age { get ; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
    }
}
