using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RimionshipServer;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Services;
using RimionshipServer.Users;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
var configuration = builder.Configuration;
void ConfigureServices(IServiceCollection services)
{
    services.Configure<RimionshipOptions>(configuration.GetSection("Rimionship"));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<RimionDbContext>(options =>
          options.UseSqlite(connectionString));
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddIdentity<RimionUser, IdentityRole>()
          .AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<RimionDbContext>()
          .AddUserManager<UserManager>()
          .AddRoleManager<RoleManager>();

    services.AddTransient<DbSeedService>()
         .AddScoped<IUserStore, UserStore>()
         .AddScoped<IUserStore<RimionUser>>(ctx => ctx.GetRequiredService<IUserStore>())
         .AddScoped<DataService>()
         .AddScoped<ConfigurationService>()
         .AddScoped<LoginService>()
         .AddSingleton<ScoreService>()
         .AddSingleton<AttentionService>();

    services.AddGrpc();

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.KnownProxies.Clear();
        options.KnownNetworks.Clear();
        options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    });

    services.AddRazorPages()
#if DEBUG
         //.AddRazorRuntimeCompilation()
#endif
          ;

    services.AddControllers();

    services.AddAuthentication()
          .AddTwitch(options =>
          {
              options.Scope.Clear();
              configuration.GetSection("Twitch").Bind(options);
          });

    services.AddSignalR()
        .AddMessagePackProtocol();
}

ConfigureServices(builder.Services);

void Configure(WebApplication app)
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
        app.UseForwardedHeaders();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseForwardedHeaders();
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();
    app.MapControllers();
    app.MapHub<DashboardHub>("/api/dashboard");
    app.MapGrpcService<GrpcService>();
}

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RimionDbContext>();
    Log.Information("Migrating databases...");
    await db.Database.MigrateAsync();
    Log.Information("Migration complete! Seeding database...");
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeedService>();
    await seeder.SeedAsync();
    Log.Information("Seeding complete!");
}

Configure(app);

await app.RunAsync();
