using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class ReportMissingPost
    {
        [Key]
        [Column("report_missing_post_id")]
        public int reportMissingPostId { get; set; }

        [Column("report_reason")]
        public string reportReason { get; set; }

        [Column("report_date")]
        public DateTime reportDate { get; set; }

        [Column("missing_post_id")]
        public int missingPostId { get; set; }

        [Column("user_id")]
        public int userId { get; set; } 



    }
}
