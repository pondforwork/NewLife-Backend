using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class AdoptionRequest
    {
        [Key]
        [Column("request_id")]
        public int RequestId { get; set; }

        [Column("adoption_post_id")]
        public int AdoptionPostId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("reason_for_adoption")]
        public string ReasonForAdoption { get; set; }

        [Column("date_added")]
        public DateTime DateAdded { get; set; }

        public virtual AdoptionPost AdoptionPost { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<NotificationAdoptionRequest> Notifications { get; set; }

    }
}
