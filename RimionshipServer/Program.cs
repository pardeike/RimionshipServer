using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using RimionshipServer.Services;
using Microsoft.Extensions.Logging;

namespace RimionshipServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using var db = new DataContext();
#if DEBUG
			Debug.WriteLine("WARNING: The database will always be deleted during startup in Debug builds.");
			_ = db.Database.EnsureDeleted();
#endif
			try
			{
				if (db.Database.EnsureCreated())
					db.CreateDefaults();
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
				.ConfigureLogging((context, builder) =>
				{
					_ = builder.ClearProviders();
					_ = builder.AddProvider(new ConsoleLoggingProvider());
				})
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
		}
	}
}
