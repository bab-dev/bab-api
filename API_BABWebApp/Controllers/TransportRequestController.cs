using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Entities.Models.Enums;
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
    [Route("api/transport-requests")]
    [ApiController]
    public class TransportRequestController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapper _repository;
        private IMapper _mapper;
        public TransportRequestController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all TransportRequests.
        /// </summary>
        /// <returns>The list of TransportRequests.</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] TransportRequestParameters parameters)
        {
            try
            {
                if (parameters.MaxRequestDate < parameters.MinRequestDate)
                {
                    return BadRequest("MaxRequestDate cannot be less than MinRequestDate");
                }

                var transportRequests = _repository.TransportRequest.GetAllTransportRequests(parameters);
                var transportRequestsResult = _mapper.Map<IEnumerable<TransportRequestDTO>>(transportRequests);
                foreach (var result in transportRequestsResult)
                {
                    foreach (var transportRequest in transportRequests)
                    {
                        if(transportRequest.Id == result.Id)
                        {
                            var volunteer = _repository.Volunteer.GetVolunteerById(transportRequest.IDCoordinator);
                            if(volunteer is not null)
                            {
                                result.CoordinatorName = _repository.Volunteer.GetVolunteerFullName(volunteer.Id);
                            }

                            var department = _repository.Department.GetDepartmentById(transportRequest.IDDepartment);
                            result.DepartmentName = department.DepartmentName;

                            result.TransportTypeName = transportRequest.TransportType.ToString();
                            result.Priority = (int)transportRequest.Priority;
                            result.Status = (int)transportRequest.Status;
                            result.PlaceTypeName = transportRequest.PlaceType.ToString();
                        }
                    }
                }
                var metadata = new
                {
                    transportRequests.TotalCount,
                    transportRequests.PageSize,
                    transportRequests.CurrentPage,
                    transportRequests.TotalPages,
                    transportRequests.HasNext,
                    transportRequests.HasPrevious
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo($"Returned all transport requests from database.");
                return Ok(transportRequestsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllTransportRequests action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private Department GetDepartment(Guid idDepartment)
        {
            Department departmentFound = _repository.Department.GetDepartmentById(idDepartment);
            return departmentFound;
        }

        private bool VolunteerExists(int idVolunteer)
        {
            Volunteer volunteerFound = _repository.Volunteer.GetVolunteerById(idVolunteer);
            return !(volunteerFound is null);
        }

        /// <summary>
        /// Gets an TransportRequest by Id.
        /// </summary>
        /// <param name="id"></param> 
        [HttpGet("{id}", Name = "TransportRequestById")]
        [Authorize]
        public IActionResult GetTransportRequestById(Guid id)
        {
            try
            {
                var transportRequest = _repository.TransportRequest.GetTransportRequestById(id);
                if (transportRequest is null)
                {
                    _logger.LogError($"Transport request with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    var transportRequestResult = _mapper.Map<TransportRequestDTO>(transportRequest);

                    var volunteer = _repository.Volunteer.GetVolunteerById(transportRequest.IDCoordinator);
                    if (volunteer is not null)
                    {
                        transportRequestResult.CoordinatorName = _repository.Volunteer.GetVolunteerFullName(volunteer.Id);
                    }

                    var department = _repository.Department.GetDepartmentById(transportRequest.IDDepartment);
                    transportRequestResult.DepartmentName = department.DepartmentName;

                    transportRequestResult.PlaceTypeName = transportRequest.PlaceType.ToString();
                    transportRequestResult.Priority = (int)transportRequest.Priority;
                    transportRequestResult.Status = (int)transportRequest.Status;

                    _logger.LogInfo($"Returned transport request with id: {id}");
                    return Ok(transportRequestResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetTransportRequestById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private string GetPlaceName(int placeType, Guid placeID)
        {
            switch (placeType)
            {
                case 0: //COMPANY
                    var company = _repository.Company.GetCompanyById(placeID);
                    if (company is not null)
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
        /// Creates a TransportRequest.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/transportRequest
        ///     {        
        ///       "IDDepartment": "32de8aef-07a5-4055-b9fe-f6d633e9a75c",
        ///       "IDCoordinator": 559744,
        ///       "Place": "Aldea SOS",
        ///       "TransportType ": 0,
        ///       "TimeRange": "9:00 AM - 11:30 AM",
        ///       "Details: "",
        ///       "Observations": "Tocar el timbre de arriba", 
        ///       "Priority": 1,
        ///       "Status": 0,
        ///       "CommentByDirector": "", 
        ///     }
        /// </remarks>
        /// <param name="transportRequest"></param>  
        [Produces("application/json")]
        [HttpPost]
        [Authorize]
        public IActionResult CreateTransportRequest([FromBody] TransportRequestForCreationAndUpdateDTO transportRequest)
        {
            try
            {
                if (transportRequest is null)
                {
                    _logger.LogError("TransportRequest object sent from client is null.");
                    return BadRequest("TransportRequest object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid transportRequest object sent from client.");
                    return BadRequest("Invalid model object");
                }
                if (!VolunteerExists(transportRequest.IDCoordinator))
                {
                    _logger.LogError($"Volunteer with id: {transportRequest.IDCoordinator}, hasn't been found in db.");
                    return BadRequest("Volunteer not found");
                }
                if (GetDepartment(transportRequest.IDDepartment) is null)
                {
                    _logger.LogError($"Department with id: {transportRequest.IDDepartment}, hasn't been found in db.");
                    return NotFound("Department not found");
                }
                var transportRequestEntity = _mapper.Map<TransportRequest>(transportRequest);
                transportRequestEntity.Place = GetPlaceName(transportRequest.PlaceType, transportRequest.IDPlace);

                _repository.TransportRequest.CreateTransportRequest(transportRequestEntity);
                _repository.Save();

                var createdTransportRequest = _mapper.Map<TransportRequestDTO>(transportRequestEntity);

                var department = GetDepartment(transportRequest.IDDepartment);
                if(department is not null)
                {
                    createdTransportRequest.DepartmentName = department.DepartmentName;
                }
                createdTransportRequest.TransportTypeName = transportRequest.TransportType.ToString();
                createdTransportRequest.Priority = (int) transportRequest.Priority;
                createdTransportRequest.Status= (int)transportRequest.Status;
                createdTransportRequest.PlaceTypeName = transportRequest.PlaceType.ToString();

                return CreatedAtRoute("TransportRequestById", new { id = createdTransportRequest.Id }, createdTransportRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateTransportRequest action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates a TransportRequest.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/transportRequest/02de8awr-07a5-4115-b9op-e7b633e9a75c
        ///     {        
        ///       "IDDepartment": "32de8aef-07a5-4055-b9fe-f6d633e9a75c",
        ///       "IDCoordinator": 559744,
        ///       "Place": "Hilando Sueños",
        ///       "TransportType ": 0,
        ///       "TimeRange": "8:00 AM - 10:30 AM",
        ///       "Details: "",
        ///       "Observations": "Tocar el timbre de arriba", 
        ///       "Priority": 2,
        ///       "Status": 0,
        ///       "CommentByDirector": "", 
        ///     }
        /// </remarks>
        /// <param name="id"></param> 
        /// <param name="transportRequest"></param>
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateTransportRequest(Guid id, [FromBody] TransportRequestForCreationAndUpdateDTO transportRequest)
        {
            try
            {
                if (transportRequest is null)
                {
                    _logger.LogError("TransportRequest object sent from client is null.");
                    return BadRequest("TransportRequest object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid transportRequest object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var transportRequestEntity = _repository.TransportRequest.GetTransportRequestById(id);
                if (transportRequestEntity is null)
                {
                    _logger.LogError($"TransportRequest with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                if (!VolunteerExists(transportRequest.IDCoordinator))
                {
                    _logger.LogError($"Volunteer with id: {transportRequest.IDCoordinator}, hasn't been found in db.");
                    return BadRequest("Volunteer not found");
                }
                if (GetDepartment(transportRequest.IDDepartment) is null)
                {
                    _logger.LogError($"Department with id: {transportRequest.IDDepartment}, hasn't been found in db.");
                    return NotFound("Department not found");
                }
                _mapper.Map(transportRequest, transportRequestEntity);
                _repository.TransportRequest.UpdateTransportRequest(transportRequestEntity);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateTransportRequest action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <param name="id"></param> 
        [HttpPut("approve/{id}", Name = "ApproveTransportRequest")]
        [Authorize]
        public IActionResult ApproveTransportRequest(Guid id)
        {
            try
            {
                var transportRequestEntity = _repository.TransportRequest.GetTransportRequestById(id);
                if (transportRequestEntity is null)
                {
                    _logger.LogError($"TransportRequest with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                transportRequestEntity.Status = StatusType.APPROVED;
                _repository.TransportRequest.UpdateTransportRequest(transportRequestEntity);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ApproveRequest action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <param name="id"></param> 
        [HttpPut("reject/{id}", Name = "RejectTransportRequest")]
        [Authorize]
        public IActionResult RejectTransportRequest(Guid id)
        {
            try
            {
                var transportRequestEntity = _repository.TransportRequest.GetTransportRequestById(id);
                if (transportRequestEntity is null)
                {
                    _logger.LogError($"TransportRequest with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                transportRequestEntity.Status = StatusType.REJECTED;
                _repository.TransportRequest.UpdateTransportRequest(transportRequestEntity);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside RejectRequest action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a TransportRequest.
        /// </summary>
        /// <param name="id"></param> 
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteTransportRequest(Guid id)
        {
            try
            {
                var transportRequest = _repository.TransportRequest.GetTransportRequestById(id);
                if (transportRequest is null)
                {
                    _logger.LogError($"TransportRequest with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _repository.TransportRequest.DeleteTransportRequest(transportRequest);
                _repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteTransportRequest action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
