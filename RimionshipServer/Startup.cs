using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RimionshipServer.Services;

namespace RimionshipServer
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
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
			_ = services.AddScoped<TokenProvider>();
			_ = services.AddScoped<UserInfoService>();

			_ = services.AddGrpc(options => options.EnableDetailedErrors = true);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				_ = app.UseDeveloperExceptionPage();
			else
				_ = app.UseExceptionHandler("/Error");

			_ = app.UseStaticFiles();
			_ = app.UseRouting();
			_ = app.UseAuthentication();
			_ = app.UseAuthorization();

			_ = app.UseEndpoints(endpoints =>
			{
				_ = endpoints.MapGrpcService<GreeterService>();

				_ = endpoints.MapBlazorHub();
				_ = endpoints.MapFallbackToPage("/_Host");
				_ = endpoints.MapControllers();
			});
		}
	}
}
