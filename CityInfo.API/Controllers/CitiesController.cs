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
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiVersion("1.0")]
	[ApiVersion("2.0")]
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

        /// <summary>
        /// Retrieves a paginated list of cities based on optional filtering criteria.
        /// </summary>
        /// <param name="name">Optional. The name of the city to filter by.</param>
        /// <param name="searchQuery">Optional. The search query to filter cities by name or other criteria.</param>
        /// <param name="pageNumber">Optional. The page number of the paginated results. Default is 1.</param>
        /// <param name="pageSize">Optional. The page size of the paginated results. Default is 10. Maximum is defined by maxCititesPageSize.</param>
        /// <returns>
        /// An HTTP response containing a paginated list of cities without their associated points of interest.
        /// </returns>
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

		/// <summary>
		/// Get a city by id.
		/// </summary>
		/// <param name="id">The id of the city to get.</param>
		/// <param name="IncludePointsOfInterest">Whether or not to include the points of interest of the city.</param>
		/// <returns>An IActionResult </returns>
		/// <response code="200">Returns the requested city</response>
	
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

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


