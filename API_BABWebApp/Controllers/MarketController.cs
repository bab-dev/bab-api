using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BABWebApp.Controllers
{
    [Route("api/markets")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public MarketController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all Markets.
        /// </summary>
        /// <returns>The list of Markets.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var markets = _repository.Market.GetAllMarkets();

                _logger.LogInfo($"Returned all markets from database.");
                return Ok(markets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMarkets action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Market by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "MarketById")]
        [Authorize]
        public IActionResult GetMarketById(Guid id)
        {
            try
            {
                var market = _repository.Market.GetMarketById(id);
                if (market is null)
                {
                    _logger.LogError($"Market with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned Market with id: {id}");
                    return Ok(market);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMarketById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a Market.
        /// </summary>
        /// <param name="market"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateMarket([FromBody] MarketForCreationAndUpdateDTO market)
        {
            try
            {
                if (market is null)
                {
                    _logger.LogError("Market object sent from client is null.");
                    return BadRequest("Market object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Market object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var marketEntity = _mapper.Map<Market>(market);
                _repository.Market.CreateMarket(marketEntity);
                _repository.Save();

                return CreatedAtRoute("MarketById", new { id = marketEntity.Id }, marketEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMarket action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Market.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="market"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateMarket(Guid id, [FromBody] MarketForCreationAndUpdateDTO market)
        {
            try
            {
                if (market is null)
                {
                    _logger.LogError("Market object sent from client is null.");
                    return BadRequest("Market object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Market object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var marketEntity = _repository.Market.GetMarketById(id);
                if (marketEntity is null)
                {
                    _logger.LogError($"Market with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                //If market name is being updated, update departure/arrival place in Trip table as well
                if (marketEntity.MarketName != market.MarketName)
                {
                    var existingTrips = _repository.Trip.FindByCondition(trip => trip.DepartureIDPlace == marketEntity.Id || trip.ArrivalIDPlace == marketEntity.Id);
                    if (existingTrips is not null)
                    {
                        foreach (var trip in existingTrips)
                        {
                            if (trip.DepartureIDPlace == marketEntity.Id)
                                trip.DeparturePlace = market.MarketName;
                            if (trip.ArrivalIDPlace == marketEntity.Id)
                                trip.ArrivalPlace = market.MarketName;

                            _repository.Trip.Update(trip);
                        }
                    }
                }

                _mapper.Map(market, marketEntity);
                _repository.Market.UpdateMarket(marketEntity);
                _repository.Save();

                return Ok(marketEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMarket action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Market.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteMarket(Guid id)
        {
            try
            {
                var Market = _repository.Market.GetMarketById(id);
                if (Market == null)
                {
                    _logger.LogError($"Market with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Market.DeleteMarket(Market);
                _repository.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMarket action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
