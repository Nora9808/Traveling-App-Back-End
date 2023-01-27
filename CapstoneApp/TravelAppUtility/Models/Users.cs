using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace TravelAppUtility.Models
{
    [Table("users")]
    public partial class Users
    {
        public Users()
        {
            Collections = new HashSet<Collections>();
            Favorites = new HashSet<Favorites>();
            Following = new HashSet<Following>();
            Reviews = new HashSet<Reviews>();
            UserEvents = new HashSet<UserEvents>();
        }

        [Key]
        [Column("user_id")]
        public int UserId { get; set; }
       
        [Column("first_name")]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Column("last_name")]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
  
        [Column("password")]
        [StringLength(100)]
        public string Password { get; set; }
        [Column("is_user")]
        public bool IsUser { get; set; }
        [Column("is_admin")]
        public bool IsAdmin { get; set; }
        [Column("date_created", TypeName = "datetime")]
        public DateTime DateCreated { get; set; }
        [Column("last_update", TypeName = "datetime")]
        public DateTime? LastUpdate { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Collections> Collections { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Favorites> Favorites { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Following> Following { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Reviews> Reviews { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserEvents> UserEvents { get; set; }
    }
}
