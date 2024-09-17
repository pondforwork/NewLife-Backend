using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class NotificationMissingPost
    {
        [Key]
        [Column("notification_id")]
        public int notificationId { get; set; }

        [Column("missing_post_id")]
        public int missingPostId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }
        
        [Column("description")]
        public string description { get; set; }

        [Column("is_read")]
        public int isRead { get; set; }

        [Column("noti_date")]
        public DateTime notiDate { get; set; }
    }
}
