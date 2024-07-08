using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NewLife_Web_api.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

}

    public DbSet<User> Users { get; set; }

    public DbSet<Post> Posts { get; set; }


}
