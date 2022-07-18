using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using RimionshipServer.Auth;
using RimionshipServer.Services;
using System.IO;

namespace RimionshipServer;

public class Startup
{
	public Startup(IWebHostEnvironment env)
	{
		var builder = new ConfigurationBuilder()
			.SetBasePath(env.ContentRootPath)
			.AddJsonFile("appsettings.json")
			.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
			.AddUserSecrets("3ca70a3b-0c13-4dd1-a7f2-5f32d3e3875a")
			.AddEnvironmentVariables();

		Configuration = builder.Build();
	}

	public IConfigurationRoot Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		//_ = services.AddLettuceEncrypt()
		//	  .PersistDataToDirectory(new DirectoryInfo("/opt/rimionship/letsencrypt"), Configuration["LettuceEncrypt:PFXPassword"]);

		_ = services.AddRazorPages();
		_ = services.AddServerSideBlazor();
		_ = services.AddControllersWithViews();

		_ = services
			.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie()
			.AddTwitch(options =>
			{
				options.Scope.Add("user:read:email");
				options.ClientId = Configuration["Twitch:ClientId"];
				options.ClientSecret = Configuration["Twitch:ClientSecret"];
				options.CallbackPath = Configuration["Twitch:CallbackPath"];

				options.ForceVerify = true;
				options.SaveTokens = true;
			});

		_ = services.AddHttpClient();
		_ = services.AddScoped<ModProvider>();
		_ = services.AddScoped<TokenProvider>();
		_ = services.AddSingleton<TwitchInfoService>();
		_ = services.AddSingleton<SyncService>();

		_ = services.AddGrpc(options =>
		{
			options.EnableDetailedErrors = true;
			// options.Interceptors.Add<UnaryInterceptor>();
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		_ = env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseExceptionHandler("/Error");

		var provider = new FileExtensionContentTypeProvider();
		provider.Mappings[".rws"] = "application/binary";
		_ = app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });

		_ = app.UseRouting();
		_ = app.UseAuthentication();
		_ = app.UseAuthorization();

		_ = app.UseEndpoints(endpoints =>
		{
			_ = endpoints.MapGrpcService<APIService>();

			_ = endpoints.MapBlazorHub();
			_ = endpoints.MapFallbackToPage("/_Host");
			_ = endpoints.MapControllers();
		});
	}
}
