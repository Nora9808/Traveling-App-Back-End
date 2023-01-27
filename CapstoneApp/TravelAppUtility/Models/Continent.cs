using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("continent")]
    public partial class Continent
    {
        public Continent()
        {
            City = new HashSet<City>();
            Country = new HashSet<Country>();
        }

        [Key]
        [Column("continent_id")]
        public int ContinentId { get; set; }
        [Required]
        [Column("continent_name")]
        [StringLength(50)]
        public string ContinentName { get; set; }
        [Required]
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [InverseProperty("Continent")]
        public virtual ICollection<City> City { get; set; }
        [InverseProperty("Continent")]
        public virtual ICollection<Country> Country { get; set; }
    }
}
