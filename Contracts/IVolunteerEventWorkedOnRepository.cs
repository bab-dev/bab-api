using Entities.DTOs.Clocking;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IVolunteerEventWorkedOnRepository
    {
        IEnumerable<VolunteerEventWorkedOn> GetAllVolunteerEventWorkedOns();
        VolunteerEventWorkedOn GetVolunteerEventWorkedOnById(Guid idVolunteerEventWorkedOn);
        VolunteerWorkStatisticsDTO GetVolunteerWorkStatisticsByIdVolunteer(int idVolunteer);
        PagedList<VolunteerWorkStatisticsDTO> GetAllVolunteerWorkStatistics([Optional] VolunteerWorkStatisticsParameters parameters);
        void CreateVolunteerEventWorkedOn(VolunteerEventWorkedOn eventWorkedOn);
        void UpdateVolunteerEventWorkedOn(VolunteerEventWorkedOn eventWorkedOn);
        void DeleteVolunteerEventWorkedOn(VolunteerEventWorkedOn eventWorkedOn);
    }
}
