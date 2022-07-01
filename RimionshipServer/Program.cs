using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace RimionshipServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using var db = new DataContext();
			Debug.WriteLine($"Database path: {db.DbPath}");
			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			 Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
	}
}
