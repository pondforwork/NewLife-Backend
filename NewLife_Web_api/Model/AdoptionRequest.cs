using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class AdoptionRequest
    {
        [Key]
        [Column("request_id")]
        public int? requestId { get; set; }

        [Column("adoption_post_id")]
        public int adoptionPostId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }

        [Column("status")]
        public string status { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("lastname")]
        public string lastname { get; set; }

        [Column("gender")]
        public string gender { get; set; }

        [Column("age")]
        public int age { get; set; }

        [Column("email")]
        public string email { get; set; }

        [Column("tel")]
        public string tel { get; set; }

        [Column("career")]
        public string career { get; set; }

        [Column("num_of_fam_members")]
        public int numOfFamMembers { get; set; }

        [Column("monthly_income")]
        public int monthlyIncome { get; set; }

        [Column("experience")]
        public string experience { get; set; }

        [Column("size_of_residence")]
        public int sizeOfResidence { get; set; }

        [Column("type_of_residence")]
        public string typeOfResidence { get; set; }

        [Column("free_time_per_day")]
        public int freeTimePerday { get; set; }

        [Column("date_added")]
        public DateTime dateAdded { get; set; }
    }
}
