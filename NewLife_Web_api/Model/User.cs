using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class User
    {
        [Key]
        [Column("user_id")] 
        public int userId { get; set; }

        [Column("profile_pic")]
        public string? profilePic { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("lastname")]
        public string lastName { get; set; }

        [Column("email")]
        public string email { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("role")]
        public string role { get; set; }


    }

}

