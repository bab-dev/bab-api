using AutoMapper;
using Contracts;
using Entities.DTOs;
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
    [Route("api/market-sellers")]
    [ApiController]
    public class MarketSellerController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public MarketSellerController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all MarketSeller suppliers.
        /// </summary>
        /// <returns>The list of MarketSeller.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] MarketSellerParameters parameters)
        {
            try
            {
                var marketSellers = _repository.MarketSeller.GetAllMarketSellers(parameters);
                var marketSellersList = new List<MarketSellerDTO>();

                foreach (var marketSeller in marketSellers)
                {
                    ProductCategory productCategory = _repository.ProductCategory.GetProductCategoryById(marketSeller.IDProductCategory);
                    if(productCategory is not null)
                    {
                        var model = _mapper.Map<MarketSellerDTO>(marketSeller);
                        model.ProductCategoryName = productCategory.ProductCategoryName;

                        marketSellersList.Add(model);
                    }
                }
                var metadata = new
                {
                    marketSellers.TotalCount,
                    marketSellers.PageSize,
                    marketSellers.CurrentPage,
                    marketSellers.TotalPages,
                    marketSellers.HasNext,
                    marketSellers.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned all marketSellers from database.");

                return Ok(marketSellersList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllMarketSellers action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a MarketSeller by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "MarketSellerById")]
        [Authorize]
        public IActionResult GetMarketSellerById(Guid id)
        {
            try
            {
                var marketSeller = _repository.MarketSeller.GetMarketSellerById(id);
                if (marketSeller is null)
                {
                    _logger.LogError($"MarketSeller with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    ProductCategory productCategory = _repository.ProductCategory.GetProductCategoryById(marketSeller.IDProductCategory);
                    var marketSellerResult = _mapper.Map<MarketSellerDTO>(marketSeller);
                    marketSellerResult.ProductCategoryName = productCategory.ProductCategoryName; 
                        
                    _logger.LogInfo($"Returned marketSeller with id: {id}");
                    return Ok(marketSellerResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMarketSelleryById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        private bool ProductCategoryExists(Guid idProductCategory)
        {
            var productCategoryFound = _repository.ProductCategory.GetProductCategoryById(idProductCategory);
            return !(productCategoryFound is null);
        }

        private bool MarketSellerAlreadyExists(MarketSellerForCreationAndUpdateDTO marketSellerDTO)
        {
            var marketSellers = _repository.MarketSeller.GetAllMarketSellers();
            var marketSellerEntity = _mapper.Map<MarketSeller>(marketSellerDTO);
            return marketSellers.Any(m => m == marketSellerEntity);
        }

        /// <summary>
        /// Creates a MarketSeller.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/marketSellers
        ///     {
        ///         "Name": "Juan Carlos",
        ///         "LastName": "Terrazas Lopez",
        ///         "MarketName": "Mercado Campesino",
        ///         "PhoneNumber": 69741054,
        ///         "IDProductCategory": "8a1919a7-9202-41ae-9eac-971sf992a589"
        ///     }
        /// </remarks>
        /// <param name="marketSeller"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateMarketSeller([FromBody] MarketSellerForCreationAndUpdateDTO marketSeller)
        {
            try
            {
                if (marketSeller is null)
                {
                    _logger.LogError("MarketSeller object sent from client is null.");
                    return BadRequest("MarketSeller object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid marketSeller object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!ProductCategoryExists(marketSeller.IDProductCategory))
                {
                    _logger.LogError($"IDProductCategory {marketSeller.IDProductCategory} was not found in database.");
                    return BadRequest("IDProductCategory was not found in database");
                }
                if (MarketSellerAlreadyExists(marketSeller))
                {
                    _logger.LogError($"MarketSeller with same data has been already registered in db.");
                    return BadRequest("MarketSeller has already been registered");
                }
                var marketSellerEntity = _mapper.Map<MarketSeller>(marketSeller);
                _repository.MarketSeller.CreateMarketSeller(marketSellerEntity);
                _repository.Save();
                return CreatedAtRoute("MarketSellerById", new { id = marketSellerEntity.Id }, marketSellerEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMarketSeller action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a MarketSeller.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/marketSellers/8a1919a7-9202-41ae-9eac-978ff992a589
        ///     {  
        ///         "Name": "Juan Carlos",
        ///         "LastName": "Terrazas Lopez",
        ///         "MarketName": "Mercado Calatayud",
        ///         "PhoneNumber": 60041054,
        ///         "IDProductCategory": "2a1919a7-9202-41ac-9eac-978ff992a501"
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="marketSeller"></param> 
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateMarketSeller(Guid id, [FromBody] MarketSellerForCreationAndUpdateDTO marketSeller)
        {
            try
            {
                if (marketSeller is null)
                {
                    _logger.LogError("MarketSeller object sent from client is null.");
                    return BadRequest("MarketSeller object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid marketSeller object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!ProductCategoryExists(marketSeller.IDProductCategory))
                {
                    _logger.LogError($"IDProductCategory {marketSeller.IDProductCategory} was not found in database.");
                    return BadRequest("IDProductCategory was not found in database");
                }
                var marketSellerEntity = _repository.MarketSeller.GetMarketSellerById(id);
                if (marketSellerEntity is null)
                {
                    _logger.LogError($"MarketSeller with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(marketSeller, marketSellerEntity);
                _repository.MarketSeller.UpdateMarketSeller(marketSellerEntity);
                _repository.Save();
                return Ok(marketSellerEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMarketSeller action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a MarketSeller.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteMarketSeller(Guid id)
        {
            try
            {
                var marketSeller = _repository.MarketSeller.GetMarketSellerById(id);
                if (marketSeller == null)
                {
                    _logger.LogError($"MarketSeller with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.MarketSeller.DeleteMarketSeller(marketSeller);
                _repository.Save();
                return Ok(marketSeller);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMarketSeller action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
