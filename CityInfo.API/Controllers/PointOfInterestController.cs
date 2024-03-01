using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Services;
using AutoMapper;
using CityInfo.API.Entities;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [Authorize(Policy = "MustBeFromAntwerp")]
    [ApiVersion("2.0")]
    [ApiController] 

    public class PointOfInterestController : ControllerBase
    {
        private readonly ILogger<PointOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointOfInterestController(ILogger<PointOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a list of points of interest for the specified city.
        /// </summary>
        /// <param name="cityId">The ID of the city for which points of interest are to be retrieved.</param>
        /// <returns>
        /// An HTTP response containing a list of points of interest for the specified city.
        /// </returns>
        [HttpGet]
        public async  Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {

            var cityName = User.Claims
               .FirstOrDefault(c => c.Type == "city")?.Value;

           if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
           {
                return Forbid();
           }

            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found.");
                return NotFound();
            }

            var pointsOfInterest = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);


                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));
            
        }

        /// <summary>
        /// Retrieves a specific point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest.</param>
        /// <returns>An HTTP response containing the requested point of interest.</returns>
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]

        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(
            int cityId, int pointOfInterestId)

        {

            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);


            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        /// <summary>
        /// Creates a new point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="pointOfInterest">The data for the new point of interest.</param>
        /// <returns>An HTTP response containing the created point of interest.</returns>
        [HttpPost]

        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId, 
            PointOfInterestForCreationDto pointOfInterest)
        {
           if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }



            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(
                cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
        }


        /// <summary>
        /// Updates an existing point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to update.</param>
        /// <param name="pointOfInterest">The updated data for the point of interest.</param>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [HttpPut("{pointOfInterestId}")]

        public async Task<ActionResult> UpdatePointOfInterest(
            int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId,pointOfInterestId);
            

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to update.</param>
        /// <param name="patchDocument">The JSON Patch document containing the changes to apply.</param>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [HttpPatch("{pointOfInterestId}")]

        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
          

            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

           var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(
               pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest();
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Deletes a specific point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest to delete.</param>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [HttpDelete("{pointOfInterestId}")]

        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
        
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send("Point Of Interest Deleted",
                $"Point Of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");  

            return NoContent();
        }

    }
}

