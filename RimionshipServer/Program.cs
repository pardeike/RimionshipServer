using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RimionshipServer.Data;
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
        .AddEntityFrameworkStores<RimionDbContext>();
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
    }
    else
    {
        app.UseExceptionHandler("/Error");
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
    Log.Information("Migration complete!");
}

Configure(app);

await app.RunAsync();