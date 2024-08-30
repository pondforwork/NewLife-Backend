using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class Image
    {
        [Key]
        [Column("image_id")]
        public int imageId { get; set; }

        [Column("image_name")]
        public string imageName { get; set; }

        [Column("image_category")]
        public string imageCategory { get; set; }

        [Column("is_processed")]
        public bool isProcessed { get; set; }

    }
}
