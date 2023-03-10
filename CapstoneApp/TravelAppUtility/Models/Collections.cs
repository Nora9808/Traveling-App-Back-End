using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("collections")]
    public partial class Collections
    {
        [Key]
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [Column("id")]
        public int? Id { get; set; }
        [Column("user_id")]
        public int? UserId { get; set; }
        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("place_id")]
        public int PlaceId { get; set; }
        [Column("place_name")]
        [StringLength(50)]
        public string PlaceName { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(Users.Collections))]
        public virtual Users User { get; set; }
    }
}
