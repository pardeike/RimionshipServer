using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data.Detailed;

namespace RimionshipServer.Data
{
    public class RimionDbContext : IdentityDbContext<RimionUser>
    {
        public DbSet<AllowedMod> AllowedMods { get; set; } = null!;

        public DbSet<BaseEntry>            BaseEntry            { get; set; } = null!;
        public DbSet<DamageTakenPawns>     DamageTakenPawns     { get; set; } = null!;
        public DbSet<DamageTakenThings>    DamageTakenThings    { get; set; } = null!;
        public DbSet<DamageDealt>          DamageDealt          { get; set; } = null!;
        public DbSet<Colonists>            Colonists            { get; set; } = null!;
        public DbSet<Wealth>               Wealth               { get; set; } = null!;
        public DbSet<MapCount>             MapCount             { get; set; } = null!;
        public DbSet<ColonistsNeedTending> ColonistsNeedTending { get; set; } = null!;
        public DbSet<MedicalConditions>    MedicalConditions    { get; set; } = null!;
        public DbSet<Enemies>              Enemies              { get; set; } = null!;
        public DbSet<WildAnimals>          WildAnimals          { get; set; } = null!;
        public DbSet<TamedAnimals>         TamedAnimals         { get; set; } = null!;
        public DbSet<Visitors>             Visitors             { get; set; } = null!;
        public DbSet<Prisoners>            Prisoners            { get; set; } = null!;
        public DbSet<DownedColonists>      DownedColonists      { get; set; } = null!;
        public DbSet<MentalColonists>      MentalColonists      { get; set; } = null!;
        public DbSet<Rooms>                Rooms                { get; set; } = null!;
        public DbSet<Caravans>             Caravans             { get; set; } = null!;
        public DbSet<WeaponDps>            WeaponDps            { get; set; } = null!;
        public DbSet<Electricity>          Electricity          { get; set; } = null!;
        public DbSet<Medicine>             Medicine             { get; set; } = null!;
        public DbSet<Food>                 Food                 { get; set; } = null!;
        public DbSet<Fire>                 Fire                 { get; set; } = null!;
        public DbSet<Conditions>           Conditions           { get; set; } = null!;
        public DbSet<Temperature>          Temperature          { get; set; } = null!;
        public DbSet<NumRaidsEnemy>        NumRaidsEnemy        { get; set; } = null!;
        public DbSet<NumThreatBigs>        NumThreatBigs        { get; set; } = null!;
        public DbSet<ColonistsKilled>      ColonistsKilled      { get; set; } = null!;
        public DbSet<GreatestPopulation>   GreatestPopulation   { get; set; } = null!;
        public DbSet<InGameHours>          InGameHours          { get; set; } = null!;

        public RimionDbContext(DbContextOptions<RimionDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AllowedMod>(entity =>
                                       {
                                           entity.HasIndex(e => e.SteamId).IsUnique();
                                           entity.HasIndex(e => e.PackageId).IsUnique();
                                       });

            builder.Entity<BaseEntry>(e =>
                                      {
                                          e.Property(x => x.Id).ValueGeneratedOnAdd();
                                          e.HasKey(x => x.UId);
                                          e.HasIndex(x => x.Id);
                                      });

            builder.Entity<DamageTakenPawns>(e =>
                                             {
                                                 e.HasOne(x => x.Id)
                                                  .WithOne()
                                                  .HasForeignKey<DamageTakenPawns>("HiddenId")
                                                  .IsRequired();
                                                 e.HasKey("HiddenId");
                                             });
            builder.Entity<DamageTakenThings>(e =>
                                              {
                                                  e.HasOne(x => x.Id)
                                                   .WithOne()
                                                   .HasForeignKey<DamageTakenThings>("HiddenId")
                                                   .IsRequired();
                                                  e.HasKey("HiddenId");
                                              });
            builder.Entity<DamageDealt>(e =>
                                        {
                                            //
                                            e.HasOne(x => x.Id)
                                             .WithOne()
                                             .HasForeignKey<DamageDealt>("HiddenId")
                                             .IsRequired();
                                            e.HasKey("HiddenId");
                                        });
            builder.Entity<Colonists>(e =>
                                      {
                                          e.HasOne(x => x.Id)
                                           .WithOne()
                                           .HasForeignKey<Colonists>("HiddenId")
                                           .IsRequired();
                                          e.HasKey("HiddenId");
                                      });
            builder.Entity<Wealth>(e =>
                                   {
                                       e.HasOne(x => x.Id)
                                        .WithOne()
                                        .HasForeignKey<Wealth>("HiddenId")
                                        .IsRequired();
                                       e.HasKey("HiddenId");
                                   });
            builder.Entity<MapCount>(e =>
                                     {
                                         e.HasOne(x => x.Id)
                                          .WithOne()
                                          .HasForeignKey<MapCount>("HiddenId")
                                          .IsRequired();
                                         e.HasKey("HiddenId");
                                     });
            builder.Entity<ColonistsNeedTending>(e =>
                                                 {
                                                     e.HasOne(x => x.Id)
                                                      .WithOne()
                                                      .HasForeignKey<ColonistsNeedTending>("HiddenId")
                                                      .IsRequired();
                                                     e.HasKey("HiddenId");
                                                 });
            builder.Entity<MedicalConditions>(e =>
                                              {
                                                  e.HasOne(x => x.Id)
                                                   .WithOne()
                                                   .HasForeignKey<MedicalConditions>("HiddenId")
                                                   .IsRequired();
                                                  e.HasKey("HiddenId");
                                              });
            builder.Entity<Enemies>(e =>
                                    {
                                        e.HasOne(x => x.Id)
                                         .WithOne()
                                         .HasForeignKey<Enemies>("HiddenId")
                                         .IsRequired();
                                        e.HasKey("HiddenId");
                                    });
            builder.Entity<WildAnimals>(e =>
                                        {
                                            e.HasOne(x => x.Id)
                                             .WithOne()
                                             .HasForeignKey<WildAnimals>("HiddenId")
                                             .IsRequired();
                                            e.HasKey("HiddenId");
                                        });
            builder.Entity<TamedAnimals>(e =>
                                         {
                                             e.HasOne(x => x.Id)
                                              .WithOne()
                                              .HasForeignKey<TamedAnimals>("HiddenId")
                                              .IsRequired();
                                             e.HasKey("HiddenId");
                                         });
            builder.Entity<Visitors>(e =>
                                     {
                                         e.HasOne(x => x.Id)
                                          .WithOne()
                                          .HasForeignKey<Visitors>("HiddenId")
                                          .IsRequired();
                                         e.HasKey("HiddenId");
                                     });
            builder.Entity<Prisoners>(e =>
                                      {
                                          e.HasOne(x => x.Id)
                                           .WithOne()
                                           .HasForeignKey<Prisoners>("HiddenId")
                                           .IsRequired();
                                          e.HasKey("HiddenId");
                                      });
            builder.Entity<DownedColonists>(e =>
                                            {
                                                e.HasOne(x => x.Id)
                                                 .WithOne()
                                                 .HasForeignKey<DownedColonists>("HiddenId")
                                                 .IsRequired();
                                                e.HasKey("HiddenId");
                                            });
            builder.Entity<MentalColonists>(e =>
                                            {
                                                e.HasOne(x => x.Id)
                                                 .WithOne()
                                                 .HasForeignKey<MentalColonists>("HiddenId")
                                                 .IsRequired();
                                                e.HasKey("HiddenId");
                                            });
            builder.Entity<Rooms>(e =>
                                  {
                                      e.HasOne(x => x.Id)
                                       .WithOne()
                                       .HasForeignKey<Rooms>("HiddenId")
                                       .IsRequired();
                                      e.HasKey("HiddenId");
                                  });
            builder.Entity<Caravans>(e =>
                                     {
                                         e.HasOne(x => x.Id)
                                          .WithOne()
                                          .HasForeignKey<Caravans>("HiddenId")
                                          .IsRequired();
                                         e.HasKey("HiddenId");
                                     });
            builder.Entity<WeaponDps>(e =>
                                      {
                                          e.HasOne(x => x.Id)
                                           .WithOne()
                                           .HasForeignKey<WeaponDps>("HiddenId")
                                           .IsRequired();
                                          e.HasKey("HiddenId");
                                      });
            builder.Entity<Electricity>(e =>
                                        {
                                            e.HasOne(x => x.Id)
                                             .WithOne()
                                             .HasForeignKey<Electricity>("HiddenId")
                                             .IsRequired();
                                            e.HasKey("HiddenId");
                                        });
            builder.Entity<Medicine>(e =>
                                     {
                                         e.HasOne(x => x.Id)
                                          .WithOne()
                                          .HasForeignKey<Medicine>("HiddenId")
                                          .IsRequired();
                                         e.HasKey("HiddenId");
                                     });
            builder.Entity<Food>(e =>
                                 {
                                     e.HasOne(x => x.Id)
                                      .WithOne()
                                      .HasForeignKey<Food>("HiddenId")
                                      .IsRequired();
                                     e.HasKey("HiddenId");
                                 });
            builder.Entity<Fire>(e =>
                                 {
                                     e.HasOne(x => x.Id)
                                      .WithOne()
                                      .HasForeignKey<Fire>("HiddenId")
                                      .IsRequired();
                                     e.HasKey("HiddenId");
                                 });
            builder.Entity<Conditions>(e =>
                                       {
                                           e.HasOne(x => x.Id)
                                            .WithOne()
                                            .HasForeignKey<Conditions>("HiddenId")
                                            .IsRequired();
                                           e.HasKey("HiddenId");
                                       });
            builder.Entity<Temperature>(e =>
                                        {
                                            e.HasOne(x => x.Id)
                                             .WithOne()
                                             .HasForeignKey<Temperature>("HiddenId")
                                             .IsRequired();
                                            e.HasKey("HiddenId");
                                        });
            builder.Entity<NumRaidsEnemy>(e =>
                                          {
                                              e.HasOne(x => x.Id)
                                               .WithOne()
                                               .HasForeignKey<NumRaidsEnemy>("HiddenId")
                                               .IsRequired();
                                              e.HasKey("HiddenId");
                                          });
            builder.Entity<NumThreatBigs>(e =>
                                          {
                                              e.HasOne(x => x.Id)
                                               .WithOne()
                                               .HasForeignKey<NumThreatBigs>("HiddenId")
                                               .IsRequired();
                                              e.HasKey("HiddenId");
                                          });
            builder.Entity<ColonistsKilled>(e =>
                                            {
                                                e.HasOne(x => x.Id)
                                                 .WithOne()
                                                 .HasForeignKey<ColonistsKilled>("HiddenId")
                                                 .IsRequired();
                                                e.HasKey("HiddenId");
                                            });
            builder.Entity<GreatestPopulation>(e =>
                                               {
                                                   e.HasOne(x => x.Id)
                                                    .WithOne()
                                                    .HasForeignKey<GreatestPopulation>("HiddenId")
                                                    .IsRequired();
                                                   e.HasKey("HiddenId");
                                               });
            builder.Entity<InGameHours>(e =>
                                        {
                                            e.HasOne(x => x.Id)
                                             .WithOne()
                                             .HasForeignKey<InGameHours>("HiddenId")
                                             .IsRequired();
                                            e.HasKey("HiddenId");
                                        });
        }
    }
}