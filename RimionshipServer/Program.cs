using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<RimionDbContext>(options =>
         options.UseSqlite(connectionString));
    services.AddDatabaseDeveloperPageExceptionFilter();

    services.AddIdentity<RimionUser, IdentityRole>()
         .AddEntityFrameworkStores<RimionDbContext>()
         .AddUserManager<UserManager>()
         .AddUserStore<UserStore>();

    services.AddTransient<DbSeedService>()
        .AddScoped<IUserStore>(ctx => ctx.GetRequiredService<UserStore>())
        .AddScoped<DataService>()
        .AddScoped<ConfigurationService>()
        .AddSingleton<ScoreService>();

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.KnownProxies.Clear();
        options.KnownNetworks.Clear();
        options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    });

    services.AddRazorPages()
#if DEBUG
        .AddRazorRuntimeCompilation()
#endif
         ;

    services.AddAuthentication()
         .AddTwitch(options =>
         {
             options.Scope.Clear();
             configuration.GetSection("Twitch").Bind(options);
         });

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
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

#if DEBUG
    app.MapControllers();
#endif
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
