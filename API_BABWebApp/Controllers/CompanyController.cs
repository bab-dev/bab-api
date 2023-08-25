using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Entities.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_BABWebApp.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public CompanyController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all Companies suppliers.
        /// </summary>
        /// <returns>The list of Companies.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] CompanyParameters companyParameters)
        {
            try
            {
                var companies = _repository.Company.GetAllCompanies(companyParameters);

                var metadata = new
                {
                    companies.TotalCount,
                    companies.PageSize,
                    companies.CurrentPage,
                    companies.TotalPages,
                    companies.HasNext,
                    companies.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned all companies from database.");

                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllCompanies action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a Company by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "CompanyById")]
        [Authorize]
        public IActionResult GetCompanyById(Guid id)
        {
            try
            {
                var company = _repository.Company.GetCompanyById(id);
                if (company is null)
                {
                    _logger.LogError($"Company with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned company with id: {id}");
                    return Ok(company);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCompanyById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool CompanyAlreadyExists(CompanyForCreationAndUpdateDTO companyDTO)
        {
            var companies = _repository.Company.GetAllCompanies();
            var companyEntity = _mapper.Map<Company>(companyDTO);
            return companies.Any(c => c == companyEntity);
        }

        /// <summary>
        /// Creates a Company.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/companies
        ///     {
        ///         "CompanyComercialName": "PIL",
        ///         "Address": "Av. Blanco Galindo Km 5",
        ///         "BusinessName": "PIL ANDINA",
        ///         "Representative": "Jorge Osorio",
        ///         "RepresentativePosition": "Gerente de Ventas",
        ///         "PhoneNumber": 69824156,
        ///         "Email": "pilandina@gmail.com"
        ///     }
        /// </remarks>
        /// <param name="company"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateCompany([FromBody] CompanyForCreationAndUpdateDTO company)
        {
            try
            {
                if (company is null)
                {
                    _logger.LogError("Company object sent from client is null.");
                    return BadRequest("Company object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid company object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (CompanyAlreadyExists(company))
                {
                    _logger.LogError($"Company with same data has been already registered in db.");
                    return BadRequest("Company has already been registered");
                }
                var companyEntity = _mapper.Map<Company>(company);
                _repository.Company.CreateCompany(companyEntity);
                _repository.Save();
                return CreatedAtRoute("CompanyById", new { id = companyEntity.Id }, companyEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCompany action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Company.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/companies/8a1919a7-9202-41ae-9eac-978ff992a589
        ///     {  
        ///         "CompanyComercialName": "PIL",
        ///         "Address": "Av. Blanco Galindo Km 10",
        ///         "BusinessName": "PIL Andina S.A",
        ///         "Representative": "Jorge Osorio",
        ///         "RepresentativePosition": "Gerente de Ventas",
        ///         "PhoneNumber": 69824122,
        ///         "Email": "pilandina.s.a@gmail.com"
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="company"></param> 
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForCreationAndUpdateDTO company)
        {
            try
            {
                if (company is null)
                {
                    _logger.LogError("Company object sent from client is null.");
                    return BadRequest("Company object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid company object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var companyEntity = _repository.Company.GetCompanyById(id);
                if (companyEntity is null)
                {
                    _logger.LogError($"Company with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
               

                //If company name is being updated, update departure/arrival place in Trip table as well
                if(companyEntity.CompanyComercialName != company.CompanyComercialName)
                {
                    var existingTrips = _repository.Trip.FindByCondition(trip => trip.DepartureIDPlace == companyEntity.Id || trip.ArrivalIDPlace == companyEntity.Id);
                    if(existingTrips is not null) {
                        foreach(var trip in existingTrips)
                        {
                            if (trip.DepartureIDPlace == companyEntity.Id)
                                trip.DeparturePlace = company.CompanyComercialName;
                            if (trip.ArrivalIDPlace == companyEntity.Id)
                                trip.ArrivalPlace = company.CompanyComercialName;

                            _repository.Trip.Update(trip);
                        }
                    }
                    
                }

                _mapper.Map(company, companyEntity);
                _repository.Company.UpdateCompany(companyEntity);
                _repository.Save();
                return Ok(companyEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCompany action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Company.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteCompany(Guid id)
        {
            try
            {
                var company = _repository.Company.GetCompanyById(id);
                if (company == null)
                {
                    _logger.LogError($"Company with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.Company.DeleteCompany(company);
                _repository.Save();
                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteCompany action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
