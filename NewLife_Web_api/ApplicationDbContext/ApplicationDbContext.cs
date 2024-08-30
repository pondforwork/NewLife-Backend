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

    public DbSet<ReportAdoptionPost> ReportAdoptionPosts { get; set; }

    public DbSet<DonationChannel> DonationChannels { get; set; }

    public DbSet<FavoriteAnimal> FavoriteAnimals { get; set; }

    public DbSet<Interest> Interests { get; set; }


    public DbSet<Breed> Breeds { get; set; }

    public DbSet<Image> Images { get; set; }


    public DbSet<ReportMissingPost> ReportMissingPosts { get; set; }

    public async Task<int?> GetSingleIntValueAsync(string tableName, string columnName)
    {
        var query = $"SELECT {columnName} as id FROM {tableName} ORDER BY id DESC";

        using (var connection = Database.GetDbConnection())
        {
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                // Execute the query and get the result
                var result = await command.ExecuteScalarAsync();

                // Convert the result to int? (nullable int)
                return result != DBNull.Value ? Convert.ToInt32(result) : (int?)null;
            }
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReportAdoptionPost>()
            .ToTable("report_adoption_post");

        modelBuilder.Entity<ReportMissingPost>()
       .ToTable("report_missing_post");


        base.OnModelCreating(modelBuilder);


    }






}
