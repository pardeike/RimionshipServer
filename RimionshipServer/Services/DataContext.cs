using Microsoft.EntityFrameworkCore;
using RimionshipServer.Models;
using System;

namespace RimionshipServer
{
	public class DataContext : DbContext
	{
		public DbSet<Participant> Participants { get; set; }
		public DbSet<Stat> Stats { get; set; }

		public string DbPath { get; }

		public DataContext()
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DbPath = System.IO.Path.Join(path, "rimionship.db");
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
