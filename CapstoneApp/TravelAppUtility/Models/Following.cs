using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("following")]
    public partial class Following
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("following_id")]
        public int? FollowingId { get; set; }
        [Column("user_id")]
        public int? UserId { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(Users.Following))]
        public virtual Users User { get; set; }
    }
}
