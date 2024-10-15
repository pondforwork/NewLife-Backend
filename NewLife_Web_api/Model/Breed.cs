using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class Breed
    {
        [Key]
        [Column("breed_id")]
        public int breedId { get; set; }

        [Column("animal_type")]
        public string animalType { get; set; }

        [Column("breed_name")]
        public string breedName { get; set; }
    }
}
