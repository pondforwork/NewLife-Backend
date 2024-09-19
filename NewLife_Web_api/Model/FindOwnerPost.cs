using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class FindOwnerPost
    {
        [Key]
        [Column("find_owner_post_id")]
        public int findOwnerPostId { get; set; }

        [Column("user_id")]
        public int userId { get; set; }

        [Column("image_1")]
        public string? Image1 { get; set; }

        [Column("image_2")]
        public string? Image2 { get; set; }
        
        [Column("image_3")]
        public string? Image3 { get; set; }

        [Column("image_4")]
        public string? Image4 { get; set; }

        [Column("image_5")]
        public string? Image5 { get; set; }

        [Column("image_6")]
        public string? Image6 { get; set; }

        [Column("image_7")]
        public string? Image7 { get; set; }

        [Column("image_8")]
        public string? Image8 { get; set; }

        [Column("image_9")]
        public string? Image9 { get; set; }

        [Column("image_10")]
        public string? Image10 { get; set; }

        [Column("name")]
        public string? name { get; set; }

        [Column("breed_id")]
        public int breedId { get; set; }

        [Column("sex")]
        public string? sex { get; set; }

        [Column("description")]
        public string? description { get; set; }

        [Column("province_id")]
        public int? province { get; set; }

        [Column("district_id")]
        public int? district { get; set; }

        [Column("subdistrict_id")]
        public int? subDistrict { get; set; }

        [Column("address_details")]
        public string? addressDetails { get; set; }

        [Column("post_status")]
        public string? postStatus { get; set; }

        [Column("create_at")]
        public DateTime? createAt { get; set; }

        [Column("update_at")]
        public DateTime? updateAt { get; set; }

        [Column("delete_at")]
        public DateTime? deleteAt { get; set; }

















    }
}
