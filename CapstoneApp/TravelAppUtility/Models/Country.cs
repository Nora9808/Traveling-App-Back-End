using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("country")]
    public partial class Country
    {
        public Country()
        {
            City = new HashSet<City>();
        }

        [Key]
        [Column("country_id")]
        public int CountryId { get; set; }
        [Column("continent_id")]
        public int? ContinentId { get; set; }
        [Required]
        [Column("country_name")]
        [StringLength(50)]
        public string CountryName { get; set; }
        [Required]
        [Column("description")]
        [StringLength(800)]
        public string Description { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey(nameof(ContinentId))]
        [InverseProperty("Country")]
        public virtual Continent Continent { get; set; }
        [InverseProperty("Country")]
        public virtual ICollection<City> City { get; set; }
    }
}
