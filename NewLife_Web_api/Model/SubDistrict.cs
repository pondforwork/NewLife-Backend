using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class SubDistrict
    {

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("zip_code")]
        public int zipCode { get; set; }

        [Column("name_th")]
        public string nameTh { get; set; }

        [Column("name_en")]
        public string nameEn { get; set; }

        [Column("district_id")]
        public int districtId { get; set; }




    }
}
