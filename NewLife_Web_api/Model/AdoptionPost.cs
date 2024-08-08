using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class AdoptionPost
    {

        [Key]
        [Column("adoption_post_id")]
        public int adoptionPostId { get; set; }

        [Column("user_id")]
        public string userId { get; set; }

        [Column("iamge_1")]
        public string? Image1 { get; set; }
       
        [Column("iamge_2")]
        public string? Image2 { get; set; }

        [Column("iamge_3")]
        public string? Image3 { get; set; }

        [Column("iamge_4")]
        public string? Image4 { get; set; }

        [Column("iamge_5")]
        public string? Image5 { get; set; }

        [Column("iamge_6")]
        public string? Image6 { get; set; }

        [Column("iamge_7")]
        public string? Image7 { get; set; }

        [Column("iamge_8")]
        public string? Image8 { get; set; }

        [Column("iamge_9")]
        public string? Image9 { get; set; }

        [Column("iamge_10")]
        public string? Image10 { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("breed_id")]
        public string breedId { get; set; }

        [Column("age")]
        public int age { get; set; }

        [Column("sex")]
        public string sex { get; set; }

        [Column("is_need_attention")]
        public string isNeedAttention { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("province_id")]
        public string provinceId { get; set; }

        [Column("district_id")]
        public string districtId { get; set; }

        [Column("subdistrict_id")]
        public string subdistrictId { get; set; }

        [Column("address_details")]
        public string addressDetails { get; set; }

        [Column("adoption_status")]
        public string adoptionStatus { get; set; }


        [Column("post_status")]
        public string postStatus { get; set; }


        [Column("create_at")]
        public string creatAt { get; set; }


        [Column("update_at")]
        public string updateAt { get; set; }

        [Column("delete_at")]
        public string deleteAt { get; set; }






    }
}
