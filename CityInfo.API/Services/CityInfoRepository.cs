using System;
using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityinfoContext _context;

        public CityInfoRepository(CityinfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            //return await _context.Cities.ToListAsync();
            return await _context.Cities.ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.Cities as IQueryable<City>;

            if(!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery)
                || (a.Description != null && a.Description.Contains(searchQuery)));
            } 

            var totalItemCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetadata(
                totalItemCount, pageSize, totalItemCount);



            var collectionToReturn =  await collection.OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointOfInterest)
        {
            if(includePointOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();

        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }



        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(
            int cityId,
            int pointOfInterestId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.Id == pointOfInterestId && p.CityId == cityId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId)
                .ToListAsync();
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);

            if(city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }


        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.Remove(pointOfInterest);
        }
    }
}

