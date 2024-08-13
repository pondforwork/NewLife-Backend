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

    public DbSet<AdoptionPost> AdoptionPosts { get; set; }

    public DbSet<NoficationAdoptionPost> NoficationAdoptionPosts { get; set; }

    public DbSet<DonationChannel> DonationChannels { get; set; }

    public DbSet<FavoriteAnimal> FavoriteAnimals { get; set; }

    public DbSet<Interest> Interests { get; set; }


    public DbSet<Breed> Breeds { get; set; }

    public DbSet<ReportMissingPost> ReportMissingPosts { get; set; }



}
