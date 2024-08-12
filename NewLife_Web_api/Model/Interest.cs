using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Interest
{
    [Key]
    [Column("interest_id")]
    public int interestId { get; set; }

    [Column("breed_id")]
    public int breedId { get; set; }

    [Column("size")]
    public string size { get; set; }

    [Column("sex")]
    public string sex { get; set; }


}
