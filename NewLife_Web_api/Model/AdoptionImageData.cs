using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewLife_Web_api.Model
{
    public class AdoptionImageData
    {
        [Key]
        [Column("adoption_image_data_id")]
        public int adoptionImageDataId { get; set; }

        [Column("adoption_image_id")]
        public int adoptionImageId { get; set; }

        [Column("image_data")]
        public string imageData { get; set; }
    }
}
