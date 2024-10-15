using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class AdoptionPostMetadata
    {
        [Key]
        [Column("adoption_post_id")]
        public int adoption_post_id { get; set; }

        [Column("animal_type")]
        public string animal_type { get; set; }

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
    }
}
