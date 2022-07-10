using Microsoft.EntityFrameworkCore;
using RimionshipServer.Models;
using System;
using System.IO;

namespace RimionshipServer.Services
{
	public class DataContext : DbContext
	{
		public DbSet<Participant> Participants { get; set; }
		public DbSet<Stat> Stats { get; set; }
		public DbSet<FutureEvent> FutureEvents { get; set; }

		public static readonly string DbPath = "/data/rimionship.db";

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			 => optionsBuilder.UseSqlite($"Data Source={DbPath}");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			_ = modelBuilder.Entity<Stat>()
				 .Property(b => b.Created)
				 .HasDefaultValueSql("getdate()");
		}
	}
}
