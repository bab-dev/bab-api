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
    [Route("api/product-categories")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public ProductCategoryController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all ProductCategories.
        /// </summary>
        /// <returns>The list of ProductCategories.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var productCategories = _repository.ProductCategory.GetAllProductCategories();
                _logger.LogInfo($"Returned all people from database.");
                return Ok(productCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetProductCategories action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a ProductCategory by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "ProductCategoryById")]
        [Authorize]
        public IActionResult GetProductCategoryById(Guid id)
        {
            try
            {
                var productCategory = _repository.ProductCategory.GetProductCategoryById(id);
                if (productCategory is null)
                {
                    _logger.LogError($"ProductCategory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ProductCategory with id: {id}");
                    return Ok(productCategory);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetProductCategoryById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a ProductCategory.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/productCategories
        ///     {        
        ///       "ProductCategoryName": "Lácteos"   
        ///     }
        /// </remarks>
        /// <param name="productCategory"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateProductCategory([FromBody] ProductCategoryForCreationAndUpdateDTO productCategory)
        {
            try
            {
                if (productCategory is null)
                {
                    _logger.LogError("Department object sent from client is null.");
                    return BadRequest("Department object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid department object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var productCategoryEntity = _mapper.Map<ProductCategory>(productCategory);
                _repository.ProductCategory.CreateProductCategory(productCategoryEntity);
                _repository.Save();
                return CreatedAtRoute("ProductCategoryById", new { id = productCategoryEntity.Id }, productCategoryEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateProductCategory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a ProductCategory.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/productCategories/17b7fc87-27da-471c-81a6-31c07bf2c80c
        ///     {        
        ///       "ProductCategoryName": "Frutas y Vegetales"    
        ///     }
        /// </remarks>
        /// <param name="id"></param> 
        /// <param name="productCategory"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateProductCategory(Guid id, [FromBody] ProductCategoryForCreationAndUpdateDTO productCategory)
        {
            try
            {
                if (productCategory is null)
                {
                    _logger.LogError("ProductCategory object sent from client is null.");
                    return BadRequest("ProductCategory object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid productCategory object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var productCategoryEntity = _repository.ProductCategory.GetProductCategoryById(id);
                if (productCategoryEntity is null)
                {
                    _logger.LogError($"ProductCategory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(productCategory, productCategoryEntity);
                _repository.ProductCategory.UpdateProductCategory(productCategoryEntity);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateProductCategory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a ProductCategory.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteProductCategory(Guid id)
        {
            try
            {
                var productCategory = _repository.ProductCategory.GetProductCategoryById(id);
                if (productCategory == null)
                {
                    _logger.LogError($"ProductCategory with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.ProductCategory.DeleteProductCategory(productCategory);
                _repository.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteProductCategory action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
