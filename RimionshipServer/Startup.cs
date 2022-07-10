using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RimionshipServer.Auth;
using RimionshipServer.Services;

namespace RimionshipServer;

public class Startup
{
	public Startup(IWebHostEnvironment env)
	{
		var builder = new ConfigurationBuilder()
			.SetBasePath(env.ContentRootPath)
			.AddJsonFile("appsettings.json")
			.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
			.AddUserSecrets(Assembly.GetExecutingAssembly())
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
		_ = services.AddScoped<UserInfoService>();

		_ = services.AddGrpc(options => options.EnableDetailedErrors = true);
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		_ = env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseExceptionHandler("/Error");

		_ = app.UseStaticFiles();
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
