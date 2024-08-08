using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class NoficationAdoptionPost
    {
        [Key]
        [Column("notification_id")]
        public int notificationId { get; set; }

        [Column("post_adoption_id")]
        public int postAdoptionId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }

        [Column("description")]
        public String description { get; set; }

        [Column("is_read")]
        public bool isRead { get; set; }

        [Column("noti_date")]
        public DateTime notiDate { get; set; }
    }
}
