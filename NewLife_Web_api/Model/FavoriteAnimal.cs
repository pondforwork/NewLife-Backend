using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class FavoriteAnimal
    {
        [Key]
        [Column("favorite_animal_id")]
        public int favoriteAnimalId { get; set; }


        [Column("user_id")]
        public int UserId { get; set; }

        [Column("adoption_post_id")]
        public int adoptionPostId { get; set; }

        [Column("date_added")]
        public DateTime dateAdded { get; set; }








    }
}
