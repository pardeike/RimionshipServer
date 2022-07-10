using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using RimionshipServer.Services;

namespace RimionshipServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using var db = new DataContext();
			Debug.WriteLine($"Database path: {db.DbPath}");
#if DEBUG
			Debug.WriteLine("WARNING: The database will always be deleted during startup in Debug builds.");
			_ = db.Database.EnsureDeleted();
#endif
			try
			{
				_ = db.Database.EnsureCreated();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
		}
	}
}
