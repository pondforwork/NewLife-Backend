using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NewLife_Web_api.Model
{
    public class User
    {
        [Key]
        [Column("user_id")]
        public int userId { get; set; }

        [Column("profile_pic")]
        public string? profilePic { get; set; }

        [Column("name")]
        public string? name { get; set; }

        [Column("lastname")]
        public string? lastName { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("password")]
        public string? password { get; set; }

        [Column("role")]
        public string? role { get; set; }

        [Column("address")]
        public string? address { get; set; }

        [Column("tel")]
        public string? tel { get; set; }

        [Column("gender")]
        public string? gender { get; set; }

        [Column("age")]
        public int? age { get; set; }

        [Column("career")]
        public string? career { get; set; }

        [Column("num_of_fam_members")]
        public int? numOfFamMembers { get; set; }

        [Column("is_have_experience")]
        public bool? isHaveExperience { get; set; }

        [Column("size_of_residence")]
        public string? sizeOfResidence { get; set; }

        [Column("type_of_residence")]
        public string? typeOfResidence { get; set; }

        [Column("free_time_per_day")]
        public int? freeTimePerDay { get; set; }

        [Column("monthly_income")]
        public int? monthlyIncome { get; set; }

        [Column("interest_id_1")]
        public int? interestId1 { get; set; }

        [Column("interest_id_2")]
        public int? interestId2 { get; set; }

        [Column("interest_id_3")]
        public int? interestId3 { get; set; }

        [Column("interest_id_4")]
        public int? interestId4 { get; set; }

        [Column("interest_id_5")]
        public int? interestId5 { get; set; }

        [JsonIgnore]
        public virtual ICollection<AdoptionRequest>? AdoptionRequests { get; set; } = new List<AdoptionRequest>();
    }

}

