using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class ReportAdoptionPost
    {
        [Key]
        [Column("report_missing_post_id")]
        public int reportMissingPostId { get; set; }

        [Column("report_reason")]
        public string reportReason { get; set; }

        [Column("report_date")]
        public DateTime reportDate { get; set; }

        [Column("adoption_post_id")]
        public int adoptionPostId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }
    }
}
