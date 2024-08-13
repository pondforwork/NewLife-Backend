using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class ReportMissingPost
    {
        [Key]
        [Column("report_missing_post")]
        public int reportMissingPost { get; set; }

        [Column("report_reason")]
        public int reportReason { get; set; }

        [Column("report_date")]
        public int reportDate { get; set; }

        [Column("missing_post_id")]
        public int missingPostId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }



    }
}
