using AutoMapper;
using Contracts;
using Entities.DTOs.BeneficiaryOrganization;
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
    [Route("api/beneficiary-organizations")]
    [ApiController]
    public class BeneficiaryOrganizationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;

        public BeneficiaryOrganizationController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the list of all Beneficiary Organizations.
        /// </summary>
        /// <returns>The list of Beneficiary Organizations.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] BeneficiaryOrganizationParameters parameters)
        {
            try
            {
                var organizations = _repository.BeneficiaryOrganization.GetAllBeneficiaryOrganizations(parameters);

                var organizationsResult = _mapper.Map<IEnumerable<BeneficiaryOrganizationDTO>>(organizations);

                var metadata = new
                {
                    organizations.TotalCount,
                    organizations.PageSize,
                    organizations.CurrentPage,
                    organizations.TotalPages,
                    organizations.HasNext,
                    organizations.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned all beneficiary organizations from database.");
                return Ok(organizationsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllBeneficiaryOrganizations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a BeneficiaryOrganization by Id.
        /// </summary>
        /// <param name="id"></param>  
        [HttpGet("{id}", Name = "BeneficiaryOrganizationById")]
        [Authorize]
        public IActionResult GetBeneficiaryOrganizationById(Guid id)
        {
            try
            {
                var organization = _repository.BeneficiaryOrganization.GetBeneficiaryOrganizationById(id);
                if (organization is null)
                {
                    _logger.LogError($"BeneficiaryOrganization with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    var organizationResult = _mapper.Map<BeneficiaryOrganizationDTO>(organization);

                    _logger.LogInfo($"Returned BeneficiaryOrganization with id: {id}");
                    return Ok(organizationResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBeneficiaryOrganizationById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool CoordinatorExists(int idCoordinator)
        {
            Volunteer coordinatorFound = _repository.Volunteer.GetVolunteerById(idCoordinator);
            return !(coordinatorFound is null);
        }

        /// <summary>
        /// Creates a Beneficiary Organization.
        /// </summary>
        /// <param name="organization"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateBeneficiaryOrganization([FromBody] BeneficiaryOrganizationForCreationAndUpdateDTO organization)
        {
            try
            {
                if (organization is null)
                {
                    _logger.LogError("Beneficiary Organization object sent from client is null.");
                    return BadRequest("Beneficiary Organization object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Beneficiary Organization object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!CoordinatorExists(organization.IDCoordinator))
                {
                    _logger.LogError($"Volunteer with id: {organization.IDCoordinator}, hasn't been found in db.");
                    return NotFound("Organization Coordinator not found");
                }

                var organizationEntity = _mapper.Map<BeneficiaryOrganization>(organization);
                _repository.BeneficiaryOrganization.CreateBeneficiaryOrganization(organizationEntity);
                _repository.Save();

                var createdBeneficiaryOrganization = _mapper.Map<BeneficiaryOrganizationDTO>(organizationEntity);
                
                return CreatedAtRoute("BeneficiaryOrganizationById", new { id = createdBeneficiaryOrganization.Id }, createdBeneficiaryOrganization);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateBeneficiaryOrganization action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a Beneficiary Organization.
        /// </summary>
        /// <param name="id"></param> 
        /// <param name="organization"></param>  
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateBeneficiaryOrganization(Guid id, [FromBody] BeneficiaryOrganizationForCreationAndUpdateDTO organization)
        {
            try
            {
                if (organization is null)
                {
                    _logger.LogError("Beneficiary Organization object sent from client is null.");
                    return BadRequest("Beneficiary Organization object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Beneficiary Organization object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!CoordinatorExists(organization.IDCoordinator))
                {
                    _logger.LogError($"Volunteer with id: {organization.IDCoordinator}, hasn't been found in db.");
                    return NotFound("Organization Coordinator not found");
                }

                var organizationEntity = _repository.BeneficiaryOrganization.GetBeneficiaryOrganizationById(id);
                if (organizationEntity is null)
                {
                    _logger.LogError($"BeneficiaryOrganization with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                //If organization name is being updated, update departure/arrival place in Trip table as well
                if (organizationEntity.OrganizationName != organization.OrganizationName)
                {
                    var existingTrips = _repository.Trip.FindByCondition(trip => trip.DepartureIDPlace == organizationEntity.Id || trip.ArrivalIDPlace == organizationEntity.Id);
                    if (existingTrips is not null)
                    {
                        foreach (var trip in existingTrips)
                        {
                            if (trip.DepartureIDPlace == organizationEntity.Id)
                                trip.DeparturePlace = organization.OrganizationName;
                            if (trip.ArrivalIDPlace == organizationEntity.Id)
                                trip.ArrivalPlace = organization.OrganizationName;

                            _repository.Trip.Update(trip);
                        }
                    }
                }

                _mapper.Map(organization, organizationEntity);
                _repository.BeneficiaryOrganization.UpdateBeneficiaryOrganization(organizationEntity);
                _repository.Save();

                return Ok(organizationEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBeneficiaryOrganization action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a Beneficiary Organization.
        /// </summary>
        /// <param name="id"></param>  
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteBeneficiaryOrganization(Guid id)
        {
            try
            {
                var organization = _repository.BeneficiaryOrganization.GetBeneficiaryOrganizationById(id);
                if (organization == null)
                {
                    _logger.LogError($"BeneficiaryOrganization with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                _repository.BeneficiaryOrganization.DeleteBeneficiaryOrganization(organization);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteBeneficiaryOrganization action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
   
    }
}
