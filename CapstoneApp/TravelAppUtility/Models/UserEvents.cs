using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("user_events")]
    public partial class UserEvents
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("event_id")]
        public int eventId { get; set; }
        [Column("user_id")]
        public int? UserId { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(Users.UserEvents))]
        public virtual Users User { get; set; }
    }
}
