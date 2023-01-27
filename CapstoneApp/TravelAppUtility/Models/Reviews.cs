using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("reviews")]
    public partial class Reviews
    {
        [Column("revews_id")]
        public int RevewsId { get; set; }

        [Column("place_id")]
        public int? placeId { get; set; }
        
        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("place_name")]
        [StringLength(50)]
        public string? placeName { get; set; }
        
        
        [Column("reviews")]
        [StringLength(500)]
        public string? Reviews1 { get; set; }
        
        
        [Column("rating", TypeName = "decimal(18, 0)")]
        public decimal? Rating { get; set; }
       
        [Column("date_created", TypeName = "datetime")]
        public DateTime? DateCreated { get; set; }
        
        
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(Users.Reviews))]
        public virtual Users User { get; set; }
    }
}
