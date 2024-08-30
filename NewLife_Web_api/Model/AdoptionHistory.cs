using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class AdoptionHistory
    {
        [Key]
        [Column("history_id")]
        public int historyId { get; set; }

        [Column("request_id")]
        public int requestId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }

        [Column("adoption_date")]
        public DateTime adoptionDate { get; set; }
    }
}
