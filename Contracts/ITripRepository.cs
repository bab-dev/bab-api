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
    public interface ITripRepository : IRepositoryBase<Trip>
    {
        PagedList<Trip> GetAllTrips([Optional] TripParameters parameters);
        Trip GetTripById(Guid idTrip);
        void CreateTrip(Trip trip);
        void UpdateTrip(Trip trip);
        void DeleteTrip(Trip trip);
    }
}
