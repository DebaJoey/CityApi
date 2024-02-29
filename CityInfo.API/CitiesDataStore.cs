using System;
using CityInfo.API.Models;

namespace CityInfo.API
{
	public class CitiesDataStore
	{
		public List<CityDto> Cities { get; set; }

		//public static CitiesDataStore Current { get;} = new CitiesDataStore(); //ensures there is one instance of CitiesDataStore Available

		public CitiesDataStore()
		{
			Cities = new List<CityDto>()
		{
			new CityDto()
			{
				Id = 1,
				Name = "New York City",
				Description = "The one with the big park.",
				PointsOfInterest = new List<PointOfInterestDto>(){
				new PointOfInterestDto()
				{
					Id = 1,
					Name = "Central Park",
					Description = "The most visited urban park in the United States."
				},

				new PointOfInterestDto()
				{
					Id = 2,
					Name = "Empire State Building",
					Description = "A 102-story kyscraper located in Midtown Manhattan"
				}
			}
			},

			new CityDto()
			{
				Id = 2,
				Name = "Antwerp",
				Description = "The one with the cathedral that really never finished.",
                PointsOfInterest = new List<PointOfInterestDto>(){
                new PointOfInterestDto()
				{
					Id = 3,
					Name = "Cathedral Of Our Lady",
					Description = "A gothic style cathedral conceived by Jan and Piete."
				},

                new PointOfInterestDto()
                {
                    Id = 4,
                    Name = "Antwerp Central Station",
                    Description = "The finest example of railway architecture in Belgium."
                }
            }
            },

			new CityDto()
			{
				Id = 3,
				Name = "Paris",
				Description = "The one with that big tower.",
                PointsOfInterest = new List<PointOfInterestDto>(){
                new PointOfInterestDto()
                {
                    Id = 5,
                    Name = "Eiffel Tower",
                    Description = "A wrought iron lattice tower on the Champ de Mars, named after"
                },

                new PointOfInterestDto()
                {
                    Id = 6,
                    Name = "The Louvre",
                    Description = "The world's largest museum."
                }
            }
            }
		};
			 }

	}
}

