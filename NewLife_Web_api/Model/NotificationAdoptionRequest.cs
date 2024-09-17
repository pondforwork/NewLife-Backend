using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class NotificationAdoptionRequest
    {
        [Key]
        [Column("noti_adop_req_id")]
        public int notiAdopReqId { get; set; }

        [Column("request_id")]
        public int requestId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }

        [Column("description")]
        public String description { get; set; }

        [Column("is_read")]
        public bool? isRead { get; set; }

        [Column("noti_date")]
        public DateTime? notiDate { get; set; }
    }
}
