using Microsoft.EntityFrameworkCore;
using RimionshipServer.Models;
using System;
using System.IO;

namespace RimionshipServer
{
	public class DataContext : DbContext
	{
		public DbSet<Participant> Participants { get; set; }
		public DbSet<Stat> Stats { get; set; }

		public string DbPath { get; }

		public DataContext()
		{
			// under docker linux DbPath will be /usr/share/database/rimionship.db
			// so we --mount type=bind,source="C:\Users\andre\Documents\Rimionship",target="/usr/share/database"

			var commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			DbPath = Path.Combine(commonAppData, "database", "rimionship.db");
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			 => options.UseSqlite($"Data Source={DbPath}");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			_ = modelBuilder.Entity<Stat>()
				 .Property(b => b.Created)
				 .HasDefaultValueSql("getdate()");
		}
	}
}
