using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class User
    {
        [Key]
        [Column("user_id")] 
        public int UserId { get; set; }

        [Column("profile_pic")]
        public string? ProfilePic { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("lastname")]
        public string LastName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("role")]
        public string Role { get; set; }
    }

}

