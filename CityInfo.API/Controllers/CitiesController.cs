using System;
using System.Text.Json;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]

	public class CitiesController : ControllerBase
	{

		private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;
		const int maxCititesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository,
			IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]

		public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities(
		  [FromQuery]	string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
		{
			if(pageSize > maxCititesPageSize)
			{
				pageSize = maxCititesPageSize;
			}

			var (cityEntities, paginationMetaData) = await _cityInfoRepository
				.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

			Response.Headers.Add("X-Pagination",
				JsonSerializer.Serialize(paginationMetaData));



			return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities));
		}


	
		[HttpGet("{id}")]

        public async Task<IActionResult> GetCity(int id, bool IncludePointsOfInterest = false)
        {
			//find city to return
			var cityToReturn = await _cityInfoRepository.GetCityAsync(id,IncludePointsOfInterest);

			if(cityToReturn == null)
			{
				return NotFound();
			}

			if(IncludePointsOfInterest)
			{
				return Ok(_mapper.Map<CityDto>(cityToReturn));
			}

			return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(cityToReturn));
        } 
    }

}


