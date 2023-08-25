using AutoMapper;
using Contracts;
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
    [Route("api/volunteer-work-statistics")]
    [ApiController]
    public class VolunteerWorkStatisticsController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IMapper _mapper;
        public VolunteerWorkStatisticsController(ILoggerManager logger, IRepositoryWrapper repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all volunteer work statistics.
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult GetAllVoluntersWorkStatistics([FromQuery] VolunteerWorkStatisticsParameters parameters)
        {
            try
            {
                var allStatistics = _repository.VolunteerEventWorkedOn.GetAllVolunteerWorkStatistics(parameters);
                if (allStatistics is null)
                {
                    _logger.LogError($"Volunteers work statistics, hasn't been found in db.");
                    return NotFound();
                }
                if (!allStatistics.Any() && parameters.IDVolunteer is not null)
                {
                    _logger.LogError($"Volunteers work statistics for that Volunteer, hasn't been found in db.");
                }
                else
                {
                    _logger.LogInfo($"Returned all volunteers work statistics");
                   
                } 

                var metadata = new
                {
                    allStatistics.TotalCount,
                    allStatistics.PageSize,
                    allStatistics.CurrentPage,
                    allStatistics.TotalPages,
                    allStatistics.HasNext,
                    allStatistics.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return Ok(allStatistics);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllVoluntersWorkStatistics action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets volunteer work statistics by IdVolunteer.
        /// </summary>
        /// <param name="idVolunteer"></param>  
        [HttpGet("{idVolunteer}", Name = "VolunterWorkStatisticsByIdVolunteer")]
        [Authorize]
        public IActionResult GetVolunterWorkStatisticsByIdVolunteer(int idVolunteer)
        {
            try
            {
                var statistics = _repository.VolunteerEventWorkedOn.GetVolunteerWorkStatisticsByIdVolunteer(idVolunteer);
                if (statistics is null)
                {
                    _logger.LogError($"Work statistics of a Volunteer with id: {idVolunteer}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned Work statistics of a Volunteer with id: {idVolunteer}");
                    return Ok(statistics);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetClockingById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
