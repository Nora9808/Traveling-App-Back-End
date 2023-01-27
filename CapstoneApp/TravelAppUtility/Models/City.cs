using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("city")]
    public partial class City
    {
        [Key]
        [Column("city_id")]
        public int CityId { get; set; }
        [Column("continent_id")]
        public int? ContinentId { get; set; }
        [Column("country_id")]
        public int? CountryId { get; set; }
        [Required]
        [Column("city_name")]
        [StringLength(50)]
        public string CityName { get; set; }
        [Required]
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey(nameof(ContinentId))]
        [InverseProperty("City")]
        public virtual Continent Continent { get; set; }
        [ForeignKey(nameof(CountryId))]
        [InverseProperty("City")]
        public virtual Country Country { get; set; }
    }
}
