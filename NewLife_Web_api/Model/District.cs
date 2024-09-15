using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewLife_Web_api.Model
{
    public class District
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public string code { get; set; }

        [Column("name_th")]
        public string nameTh { get; set; }

        [Column("name_en")]
        public string nameEn { get; set; }

        [Column("province_id")]
        public int provinceId { get; set; }
    }
}
