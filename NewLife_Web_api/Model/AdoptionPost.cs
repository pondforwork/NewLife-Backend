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
        public int userId { get; set; }

        [Column("image_1_id")]
        public int? Image1 { get; set; }
       
        [Column("image_2_id")]
        public int? Image2 { get; set; }

        [Column("image_3_id")]
        public int? Image3 { get; set; }

        [Column("image_4_id")]
        public int? Image4 { get; set; }

        [Column("image_5_id")]
        public int? Image5 { get; set; }

        [Column("image_6_id")]
        public int? Image6 { get; set; }

        [Column("image_7_id")]
        public int? Image7 { get; set; }

        [Column("image_8_id")]
        public int? Image8 { get; set; }

        [Column("image_9_id")]
        public int? Image9 { get; set; }

        [Column("image_10_id")]
        public int? Image10 { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("breed_id")]
        public int breedId { get; set; }

        [Column("age")]
        public int age { get; set; }

        [Column("sex")]
        public string sex { get; set; }

        [Column("is_need_attention")]
        public bool isNeedAttention { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("province_id")]
        public int provinceId { get; set; }

        [Column("district_id")]
        public int districtId { get; set; }

        [Column("subdistrict_id")]
        public int subdistrictId { get; set; }

        [Column("address_details")]
        public string addressDetails { get; set; }

        [Column("adoption_status")]
        public string adoptionStatus { get; set; }


        [Column("post_status")]
        public string postStatus { get; set; }


        [Column("create_at")]
        public DateTime? creatAt { get; set; }


        [Column("update_at")]
        public DateTime? updateAt { get; set; }

        [Column("delete_at")]
        public DateTime? deleteAt { get; set; }






    }
}
