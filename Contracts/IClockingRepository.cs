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
    public interface IClockingRepository : IRepositoryBase<Clocking>
    {
        PagedList<Clocking> GetAllClockings([Optional] ClockingParameters parameters);
        Clocking GetClockingById(Guid idClocking);
        Clocking GetClockingByIdVolunteerAndDate(int idVolunteer, DateTime date);
        void CreateClocking(Clocking clocking);
        void UpdateClocking(Clocking clocking);
        void DeleteClocking(Clocking clocking);
    }
}
