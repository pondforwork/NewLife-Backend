using System.ComponentModel.DataAnnotations;

public class Post
{
    [Key]
    public int locationId { get; set; }

    
    public string Name { get; set; }
    public string Code { get; set; }
}
