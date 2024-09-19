using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class MissingPost
    {
        [Column("missing_post_id")]
        public int missingPostId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }

        [Column("image_1")]
        public string image_1 { get; set; }

        [Column("image_2")]
        public string image_2 { get; set; }

        [Column("image_3")]
        public string image_3 { get; set; }

        [Column("image_4")]
        public string? image_4 { get; set; }

        [Column("image_5")]
        public string? image_5 { get; set; }

        [Column("image_6")]
        public string? image_6 { get; set; }

        [Column("image_7")]
        public string? image_7 { get; set; }

        [Column("image_8")]
        public string? image_8 { get; set; }

        [Column("image_9")]
        public string? image_9 { get; set; }

        [Column("image_10")]
        public string? image_10 { get; set; }

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
        public int subDistrictId { get; set; }

        [Column("address_details")]
        public string addressDetails { get; set; }

        [Column("post_code")]
        public int postCode { get; set; }

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
