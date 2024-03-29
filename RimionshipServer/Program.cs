﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
#if RELEASE
using NUglify.Css;
using NUglify.JavaScript;
using NUglify;
#endif
using RimionshipServer;
using RimionshipServer.API;
using RimionshipServer.Data;
using RimionshipServer.Pages.Api;
using RimionshipServer.Services;
using RimionshipServer.Users;
using Serilog;
using WebMarkupMin.AspNetCore6;
using WebMarkupMin.NUglify;
using CssColor = WebMarkupMin.NUglify.CssColor;
using CssComment = WebMarkupMin.NUglify.CssComment;

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
        .AddScoped<IAuthorizationHandler, CustomRoleAuthHandler>()
        .AddSingleton<ScoreService>()
        .AddSingleton<EventsService>()
        .AddSingleton<AttentionService>()
        .AddSingleton<DirectionService>()
        .AddSingleton<SettingService>()
        .AddHostedService<EventsServiceCulling>();

    services.AddGrpc();

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.KnownProxies.Clear();
        options.KnownNetworks.Clear();
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    services.AddRazorPages(options =>
    {
        options.RootDirectory = "/Pages";
        options.Conventions.AuthorizeFolder("/Admin", Roles.Admin);
        options.Conventions.AddPageRoute("/API/SimpleGraph", "/API/EmbedGraph/{statt}/{username}/{width}/{height}/{handler=Embed}");
    })
#if DEBUG
        .AddRazorRuntimeCompilation()
#endif
        ;

    services.AddControllers();

    services.AddAuthentication()
        .AddTwitch(options =>
        {
            options.Scope.Clear();
            if (builder.Environment.IsDevelopment())
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            configuration.GetSection("Twitch").Bind(options);
        });

    services.AddSignalR()
        .AddMessagePackProtocol();

    services.AddAuthorization(options =>
    {
        options.AddPolicy(Roles.Admin, policyBuilder =>
        {
            policyBuilder.AddRequirements(new CustomRoleAuth(Roles.Admin));
        });
    });

    services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = true;
        options.AppendTrailingSlash = true;
    });
    services.AddWebMarkupMin(
                             options =>
                             {
                                 options.DisablePoweredByHttpHeaders = true;
                             })
            .AddHtmlMinification(
                                 options =>
                                 {
                                     options.JsMinifierFactory = new NUglifyJsMinifierFactory(new NUglifyJsMinificationSettings
                                     {
                                         PreserveImportantComments = false
                                     });
                                     options.CssMinifierFactory = new NUglifyCssMinifierFactory(new NUglifyCssMinificationSettings
                                     {
                                         ColorNames = CssColor.Major,
                                         CommentMode = CssComment.None
                                     });
                                     options.MinificationSettings.RemoveRedundantAttributes = true;
                                     options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                                     options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
                                 })
            .AddHttpCompression();
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
    app.UseWebMarkupMin();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseStatusCodePagesWithRedirects("~/Error/{0}");
    app.MapRazorPages();
    app.MapControllers();
    app.MapHub<DashboardHub>("/api/dashboard");
    app.MapGrpcService<GrpcService>();
}

var app = builder.Build();

#if RELEASE
Log.Information("Minifying Javascript");
await Minify(Path.Combine("wwwroot", "js"), "js");
Log.Information("Minifying Javascript done!");
Log.Information("Minifying Cascading Style Sheets");
await Minify(Path.Combine("wwwroot", "css"), "css");
Log.Information("Minifying Cascading Style Sheets done!");
async Task Minify(string path, string type)
{
    if (type is not "js" and not "css")
        throw new ArgumentException(type);
    var dirs = Directory.GetDirectories(path);
    await Task.WhenAll(dirs.Select(async x => await Minify(x, type)));
    var files = Directory.GetFiles(path);
    foreach (string file in files)
    {
        if (file.EndsWith(".min." +type) || !file.EndsWith("."+type))
        {
            continue;
        }
        Log.Information("Found Matching Source: {File}", file);
        var siteJs = await File.ReadAllTextAsync(file);
        var result = type switch{
                         "js"  => Uglify.Js(siteJs, file, new CodeSettings{
                                                                              PreserveImportantComments = false
                                                                          }),
                         "css" => Uglify.Css(siteJs, file, new CssSettings{
                                                                              ColorNames  = NUglify.Css.CssColor.Major,
                                                                              CommentMode = NUglify.Css.CssComment.None   
                                                                          }, 
                                             new CodeSettings{
                                                                 PreserveImportantComments = false
                                                             }),
                         _     => throw new ArgumentException(type)
                     };
        if (!result.HasErrors)
        {
            var newFile = file.Replace("." +type, ".min." +type);
            Log.Information("Processed Source: {File} → {NewFile}", file, newFile);
            await File.WriteAllTextAsync(newFile, result.Code);
        }
        else
        {
            foreach (UglifyError resultError in result.Errors)
            {
                Log.Error("Error while minifying: {Error}", resultError);
            }
        }
    }
}
#endif

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RimionDbContext>();
    Log.Information("Migrating databases...");
    await db.Database.MigrateAsync();
    Log.Information("Migration complete! Seeding database...");
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeedService>();
    await seeder.SeedAsync();
    Log.Information("Seeding complete! Fetching persistent inMemory-Data");
    await Task.WhenAll(Stats.InitStatFromDatabase(db));
    var settingsAsync = await db.GetSaveSettingsAsync();
    if (settingsAsync.SaveFile is not null)
    {
        SaveFilePageModel.SafeFile = await db.SaveFiles.AsNoTracking().Where(x => x.Name == settingsAsync.SaveFile).FirstAsync();
    }
    Log.Information("Fetching Complete!");
}

Configure(app);

await app.RunAsync();
