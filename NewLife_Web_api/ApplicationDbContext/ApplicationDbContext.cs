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

    public DbSet<ReportMissingPost> ReportMissingPosts { get; set; }

    public DbSet<AdoptionHistory> AdoptionHistorys { get; set; }

    public DbSet<Provinces> Provincess { get; set; }

    public DbSet<AdoptionRequest> AdoptionRequest { get; set; }

    public DbSet<SubDistrict> SubDistricts { get; set; }

    public DbSet<District> Districts { get; set; }

    public DbSet<NotificationMissingPost> NotificationMissingPosts { get; set; }

    public DbSet<NotificationAdoptionRequest> NotificationAdoptionRequests { get; set; }

    public DbSet<FindOwnerPost> FindOwnerPosts { get; set; }

    public DbSet<MissingPost> MissingPosts { get; set; }

    public DbSet<AdoptionPostMetadata> AdoptionPostMetadata { get; set; }

    public DbSet<AdoptionImage> AdoptionImage { get; set; }
    public DbSet<AdoptionImageData> AdoptionImageData { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

     

        modelBuilder.Entity<Breed>(entity =>
        {
            entity.ToTable("breed");
            entity.HasKey(e => e.breedId);
        });

        modelBuilder.Entity<ReportAdoptionPost>()
            .ToTable("report_adoption_post");

        modelBuilder.Entity<ReportMissingPost>()
       .ToTable("report_missing_post");

        modelBuilder.Entity<AdoptionHistory>()
      .ToTable("adoption_history");

        modelBuilder.Entity<Provinces>()
     .ToTable("provinces");
       
        modelBuilder.Entity<SubDistrict>()
     .ToTable("sub_district");

        modelBuilder.Entity<NotificationMissingPost>()
    .ToTable("notification_missing_post");

        modelBuilder.Entity<NotificationAdoptionRequest>()
    .ToTable("notification_adoption_request");

        modelBuilder.Entity<MissingPost>()
    .ToTable("missing_post");

        modelBuilder.Entity<User>()
    .ToTable("user");

        modelBuilder.Entity<AdoptionPost>(entity =>
        {
            entity.ToTable("adoption_post");  // กำหนดชื่อตารางให้ตรงกับในฐานข้อมูล
            entity.HasKey(e => e.adoptionPostId);  // กำหนดคีย์หลักของตาราง
        });

        modelBuilder.Entity<AdoptionRequest>(entity =>
        {
            entity.ToTable("adoption_request");
            entity.HasKey(e => e.RequestId);
            entity.HasOne(e => e.User)
                .WithMany(u => u.AdoptionRequests)
                .HasForeignKey(e => e.UserId);
            entity.HasOne(e => e.AdoptionPost)
                .WithMany()
                .HasForeignKey(e => e.AdoptionPostId);
        });

        modelBuilder.Entity<NotificationAdoptionRequest>(entity =>
        {
            entity.ToTable("notification_adoption_request");
            entity.HasKey(e => e.NotiAdopReqId);
            entity.HasOne(n => n.AdoptionRequest)
                .WithMany(ar => ar.Notifications)
                .HasForeignKey(n => n.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        base.OnModelCreating(modelBuilder);


    }
}
