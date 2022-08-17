using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data.Detailed;

namespace RimionshipServer.Data
{
    public class RimionDbContext : IdentityDbContext<RimionUser>
    {
        public DbSet<AllowedMod> AllowedMods { get; set; } = null!;

        public DbSet<BaseEntry>            BaseEntry            {get; set;} = null!;
        public DbSet<DamageTakenPawns>     DamageTakenPawns     {get; set;} = null!;
        public DbSet<DamageTakenThings>    DamageTakenThings    {get; set;} = null!;
        public DbSet<DamageDealt>          DamageDealt          {get; set;} = null!;
        public DbSet<Colonists>            Colonists            {get; set;} = null!;
        public DbSet<Wealth>               Wealth               {get; set;} = null!;
        public DbSet<MapCount>             MapCount             {get; set;} = null!;
        public DbSet<ColonistsNeedTending> ColonistsNeedTending {get; set;} = null!;
        public DbSet<MedicalConditions>    MedicalConditions    {get; set;} = null!;
        public DbSet<Enemies>              Enemies              {get; set;} = null!;
        public DbSet<WildAnimals>          WildAnimals          {get; set;} = null!;
        public DbSet<TamedAnimals>         TamedAnimals         {get; set;} = null!;
        public DbSet<Visitors>             Visitors             {get; set;} = null!;
        public DbSet<Prisoners>            Prisoners            {get; set;} = null!;
        public DbSet<DownedColonists>      DownedColonists      {get; set;} = null!;
        public DbSet<MentalColonists>      MentalColonists      {get; set;} = null!;
        public DbSet<Rooms>                Rooms                {get; set;} = null!;
        public DbSet<Caravans>             Caravans             {get; set;} = null!;
        public DbSet<WeaponDps>            WeaponDps            {get; set;} = null!;
        public DbSet<Electricity>          Electricity          {get; set;} = null!;
        public DbSet<Medicine>             Medicine             {get; set;} = null!;
        public DbSet<Food>                 Food                 {get; set;} = null!;
        public DbSet<Fire>                 Fire                 {get; set;} = null!;
        public DbSet<Conditions>           Conditions           {get; set;} = null!;
        public DbSet<Temperature>          Temperature          {get; set;} = null!;
        public DbSet<NumRaidsEnemy>        NumRaidsEnemy        {get; set;} = null!;
        public DbSet<NumThreatBigs>        NumThreatBigs        {get; set;} = null!;
        public DbSet<ColonistsKilled>      ColonistsKilled      {get; set;} = null!;
        public DbSet<GreatestPopulation>   GreatestPopulation   {get; set;} = null!;
        public DbSet<InGameHours>          InGameHours          {get; set;} = null!;
        
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
                                                 e.HasKey(x => x.Id);
                                                 e.HasOne(x => x.Id);
                                             });

            builder.Entity<DamageTakenPawns>(e =>
                                             {
                                                 e.HasKey(x => x.Id);
                                                 e.HasOne(x => x.Id);
                                             });
            builder.Entity<DamageTakenThings>(e =>
                                              {
                                                  e.HasKey(x => x.Id);
                                                  e.HasOne(x => x.Id);
                                              });
            builder.Entity<DamageDealt>(e =>
                                        {
                                            e.HasKey(x => x.Id);
                                            e.HasOne(x => x.Id);
                                        });
            builder.Entity<Colonists>(e =>
                                      {
                                          e.HasKey(x => x.Id);
                                          e.HasOne(x => x.Id);
                                      });
            builder.Entity<Wealth>(e =>
                                   {
                                       e.HasKey(x => x.Id);
                                       e.HasOne(x => x.Id);
                                   });
            builder.Entity<MapCount>(e =>
                                     {
                                         e.HasKey(x => x.Id);
                                         e.HasOne(x => x.Id);
                                     });
            builder.Entity<ColonistsNeedTending>(e =>
                                                 {
                                                     e.HasKey(x => x.Id);
                                                     e.HasOne(x => x.Id);
                                                 });
            builder.Entity<MedicalConditions>(e =>
                                              {
                                                  e.HasKey(x => x.Id);
                                                  e.HasOne(x => x.Id);
                                              });
            builder.Entity<Enemies>(e =>
                                    {
                                        e.HasKey(x => x.Id);
                                        e.HasOne(x => x.Id);
                                    });
            builder.Entity<WildAnimals>(e =>
                                        {
                                            e.HasKey(x => x.Id);
                                            e.HasOne(x => x.Id);
                                        });
            builder.Entity<TamedAnimals>(e =>
                                         {
                                             e.HasKey(x => x.Id);
                                             e.HasOne(x => x.Id);
                                         });
            builder.Entity<Visitors>(e =>
                                     {
                                         e.HasKey(x => x.Id);
                                         e.HasOne(x => x.Id);
                                     });
            builder.Entity<Prisoners>(e =>
                                      {
                                          e.HasKey(x => x.Id);
                                          e.HasOne(x => x.Id);
                                      });
            builder.Entity<DownedColonists>(e =>
                                            {
                                                e.HasKey(x => x.Id);
                                                e.HasOne(x => x.Id);
                                            });
            builder.Entity<MentalColonists>(e =>
                                            {
                                                e.HasKey(x => x.Id);
                                                e.HasOne(x => x.Id);
                                            });
            builder.Entity<Rooms>(e =>
                                  {
                                      e.HasKey(x => x.Id);
                                      e.HasOne(x => x.Id);
                                  });
            builder.Entity<Caravans>(e =>
                                     {
                                         e.HasKey(x => x.Id);
                                         e.HasOne(x => x.Id);
                                     });
            builder.Entity<WeaponDps>(e =>
                                      {
                                          e.HasKey(x => x.Id);
                                          e.HasOne(x => x.Id);
                                      });
            builder.Entity<Electricity>(e =>
                                        {
                                            e.HasKey(x => x.Id);
                                            e.HasOne(x => x.Id);
                                        });
            builder.Entity<Medicine>(e =>
                                     {
                                         e.HasKey(x => x.Id);
                                         e.HasOne(x => x.Id);
                                     });
            builder.Entity<Food>(e =>
                                 {
                                     e.HasKey(x => x.Id);
                                     e.HasOne(x => x.Id);
                                 });
            builder.Entity<Fire>(e =>
                                 {
                                     e.HasKey(x => x.Id);
                                     e.HasOne(x => x.Id);
                                 });
            builder.Entity<Conditions>(e =>
                                       {
                                           e.HasKey(x => x.Id);
                                           e.HasOne(x => x.Id);
                                       });
            builder.Entity<Temperature>(e =>
                                        {
                                            e.HasKey(x => x.Id);
                                            e.HasOne(x => x.Id);
                                        });
            builder.Entity<NumRaidsEnemy>(e =>
                                          {
                                              e.HasKey(x => x.Id);
                                              e.HasOne(x => x.Id);
                                          });
            builder.Entity<NumThreatBigs>(e =>
                                          {
                                              e.HasKey(x => x.Id);
                                              e.HasOne(x => x.Id);
                                          });
            builder.Entity<ColonistsKilled>(e =>
                                            {
                                                e.HasKey(x => x.Id);
                                                e.HasOne(x => x.Id);
                                            });
            builder.Entity<GreatestPopulation>(e =>
                                               {
                                                   e.HasKey(x => x.Id);
                                                   e.HasOne(x => x.Id);
                                               });
            builder.Entity<InGameHours>(e =>
                                        {
                                            e.HasKey(x => x.Id);
                                            e.HasOne(x => x.Id);
                                        });
        }
    }
}