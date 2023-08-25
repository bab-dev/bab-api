using AutoMapper;
using Contracts;
using Entities.DTOs.Trip;
using Entities.Models;
using Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/trips")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public TripController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all Trips.
        /// </summary>
        /// <returns>The list of Trips.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] TripParameters parameters)
        {
            try
            {
                var trips = _repository.Trip.GetAllTrips(parameters);

                var tripsResult = new List<TripDTO>();

                foreach(var trip in trips)
                {
                    var model = _mapper.Map<TripDTO>(trip);

                    model.DepartureTime = trip.DepartureTime.ToString(@"hh\:mm");
                    model.ArrivalTime = trip.ArrivalTime.ToString(@"hh\:mm");
                    model.DepartureTypeName = trip.DepartureType.ToString();
                    model.ArrivalTypeName = trip.ArrivalType.ToString();
                    model.TransportTypeName = trip.TransportType.ToString();

                    tripsResult.Add(model);
                }

                var metadata = new
                {
                    trips.TotalCount,
                    trips.PageSize,
                    trips.CurrentPage,
                    trips.TotalPages,
                    trips.HasNext,
                    trips.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned all people from database.");
                return Ok(tripsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetTrips action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Trip by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "TripById")]
        [Authorize]
        public IActionResult GetTripById(Guid id)
        {
            try
            {
                var trip = _repository.Trip.GetTripById(id);
                if (trip is null)
                {
                    _logger.LogError($"Trip with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    var tripResult = _mapper.Map<TripDTO>(trip);
                    tripResult.DepartureTime = trip.DepartureTime.ToString(@"hh\:mm");
                    tripResult.ArrivalTime = trip.ArrivalTime.ToString(@"hh\:mm");
                    tripResult.DepartureTypeName = trip.DepartureType.ToString();
                    tripResult.ArrivalTypeName = trip.ArrivalType.ToString();
                    tripResult.TransportTypeName = trip.TransportType.ToString();
                    
                    _logger.LogInfo($"Returned Trip with id: {id}");
                    return Ok(tripResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetTripById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private string GetPlaceName(int placeType, Guid placeID)
        {
            switch (placeType)
            {
                case 0: //COMPANY
                    var company = _repository.Company.GetCompanyById(placeID);
                    if(company is not null)
                        return company.CompanyComercialName;
                    else
                        return "";
                case 1: // MARKET,
                    var market = _repository.Market.GetMarketById(placeID);
                    if (market is not null)
                        return market.MarketName;
                    else
                        return "";
                case 2: // BENEFICIARY_ORGANIZATION
                    var organization = _repository.BeneficiaryOrganization.GetBeneficiaryOrganizationById(placeID);
                    if (organization is not null)
                        return organization.OrganizationName;
                    else
                        return "";
                case 3: // FOOD_BANK,
                    return "Almacén BAB";
                case 4: // HYDROPONIC_PLANT,
                    return "Planta de Hidroponía";
                default:
                    return "";

            }
        }

        /// <summary>
        /// Creates a Trip.
        /// </summary>
        /// <param name="trip"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateTrip([FromBody] TripForCreationAndUpdateDTO trip)
        {
            try
            {
                if (trip is null)
                {
                    _logger.LogError("Trip object sent from client is null.");
                    return BadRequest("Trip object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Trip object sent from client.");
                    return BadRequest("Invalid model object");
                }
                

                var tripEntity = _mapper.Map<Trip>(trip);
                tripEntity.DepartureTime = TimeSpan.Parse(trip.DepartureTime);
                tripEntity.ArrivalTime = TimeSpan.Parse(trip.ArrivalTime);

                tripEntity.DeparturePlace = GetPlaceName(trip.DepartureType, trip.DepartureIDPlace);
                tripEntity.ArrivalPlace = GetPlaceName(trip.ArrivalType, trip.ArrivalIDPlace);

                _repository.Trip.CreateTrip(tripEntity);
                _repository.Save();

                var createdTrip = _mapper.Map<TripDTO>(tripEntity);
                createdTrip.DepartureTime = tripEntity.DepartureTime.ToString(@"hh\:mm");
                createdTrip.ArrivalTime = tripEntity.ArrivalTime.ToString(@"hh\:mm");
                createdTrip.DepartureTypeName = tripEntity.DepartureType.ToString();
                createdTrip.ArrivalTypeName = tripEntity.ArrivalType.ToString();
                createdTrip.TransportTypeName = tripEntity.TransportType.ToString();

                return CreatedAtRoute("TripById", new { id = createdTrip.Id }, createdTrip);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateTrip action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Trip.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="trip"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateTrip(Guid id, [FromBody] TripForCreationAndUpdateDTO trip)
        {
            try
            {
                if (trip is null)
                {
                    _logger.LogError("Trip object sent from client is null.");
                    return BadRequest("Trip object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Trip object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var tripEntity = _repository.Trip.GetTripById(id);
                if (tripEntity is null)
                {
                    _logger.LogError($"Trip with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(trip, tripEntity);
                tripEntity.DepartureTime = TimeSpan.Parse(trip.DepartureTime);
                tripEntity.ArrivalTime = TimeSpan.Parse(trip.ArrivalTime);
                tripEntity.DeparturePlace = GetPlaceName(trip.DepartureType, trip.DepartureIDPlace);
                tripEntity.ArrivalPlace = GetPlaceName(trip.ArrivalType, trip.ArrivalIDPlace);

                _repository.Trip.UpdateTrip(tripEntity);
                _repository.Save();
                return Ok(tripEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateTrip action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Trip.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteTrip(Guid id)
        {
            try
            {
                var trip = _repository.Trip.GetTripById(id);
                if (trip == null)
                {
                    _logger.LogError($"Trip with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Trip.DeleteTrip(trip);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteTrip action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
