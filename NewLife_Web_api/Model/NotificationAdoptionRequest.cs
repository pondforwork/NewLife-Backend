using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class NotificationAdoptionRequest
    {
        [Key]
        [Column("noti_adop_req_id")]
        public int NotiAdopReqId { get; set; }

        [Column("request_id")]
        public int RequestId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; }

        [Column("noti_date")]
        public DateTime NotiDate { get; set; }

        public virtual AdoptionRequest AdoptionRequest { get; set; }
        public virtual User User { get; set; }

    }
}
