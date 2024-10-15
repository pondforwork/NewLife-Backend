using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class AdoptionImage
    {
        [Key]
        [Column("adoption_image_id")]
        public int adoptionImageId { get; set; }

        [Column("image_name")]
        public string imageName { get; set; }

    }
}
