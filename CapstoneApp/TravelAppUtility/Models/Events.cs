using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("events")]
    public partial class Events
    {
        [Key]
        [Column("events_id")]
        public int EventsId { get; set; }
        [Required]
        [Column("user_id")]
        public int userId { get; set; }
        [Required]
        [Column("event")]
        [StringLength(100)]
        public string Event { get; set; }
        [Required]
        [Column("location")]
        [StringLength(300)]
        public string Location { get; set; }
        [Required]
        [Column("details")]
        [StringLength(500)]
        public string Details { get; set; }
        [Column("start_date", TypeName = "date")]
        public DateTime StartDate { get; set; }
        [Column("end_date", TypeName = "date")]
        public DateTime EndDate { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }
    }
}
