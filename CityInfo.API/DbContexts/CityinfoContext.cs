using System;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
namespace CityInfo.API.DbContexts
{
	public class CityinfoContext : DbContext
	{
        public CityinfoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; } = null!;

		public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one with the big park."
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "The one with the cathedral that really never finished."
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one with that big tower."
                }
                );

            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The most visited urban park in the United States."
                },
                    new PointOfInterest("Empire State Building")
                    {
                        Id = 2,
                        CityId = 1,
                        Description = "A 102-story kyscraper located in Midtown Manhattan"
                    },
                    new PointOfInterest("Cathedral Of Our Lady")
                    {
                        Id = 3,
                        CityId = 2,
                        Description = "A gothic style cathedral conceived by Jan and Piete."
                    },
                    new PointOfInterest("Antwerp Central Station")
                    {
                        Id = 4,
                        CityId = 2,
                        Description = "The finest example of railway architecture in Belgium."
                    },
                    new PointOfInterest("Eiffel Tower")
                    {
                        Id = 5,
                        CityId = 3,
                        Description = "A wrought iron lattice tower on the Champ de Mars, named after"
                    },
                    new PointOfInterest("The Louvre")
                    {
                        Id = 6,
                        CityId = 3,
                        Description = "The world's largest museum."
                    }
                    );

            base.OnModelCreating(modelBuilder);
        }
        
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //	optionsBuilder.UseSqlite("connectionstring");
        //	base.OnConfiguring(optionsBuilder);
        //}
    }
}

