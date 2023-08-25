using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TripRepository : RepositoryBase<Trip>, ITripRepository
    {
        private readonly ISortHelper<Trip> _sortHelper;
        public TripRepository(RepositoryContext repositoryContext, ISortHelper<Trip> sortHelper)
            : base(repositoryContext)
        {
            _sortHelper = sortHelper;
        }
        private void SearchByPlace(ref IQueryable<Trip> trips, string place)
        {
            if (!trips.Any() || string.IsNullOrWhiteSpace(place))
                return;

            trips = trips.Where(trip =>
                trip.DeparturePlace.ToLower().Contains(place.Trim().ToLower()) ||
                trip.ArrivalPlace.ToLower().Contains(place.Trim().ToLower()));
        }

        public PagedList<Trip> GetAllTrips([Optional] TripParameters parameters)
        {
            IQueryable <Trip> trips = FindByCondition(trip =>
                ((trip.IDCoordinator == parameters.IDCoordinator || parameters.IDCoordinator == null)
                    && (trip.IDDepartment == parameters.IDDepartment || parameters.IDDepartment == null)
                    && (trip.Vehicule == parameters.Vehicule || parameters.Vehicule == null)
                    && (trip.Date.Month == parameters.Month || parameters.Month == null)
                    && (trip.Date.Year == parameters.Year || parameters.Year == null)
                    && ((int)trip.TransportType == parameters.TransportType || parameters.TransportType == null)
                    && (trip.DeparturePlace == parameters.DeparturePlace || parameters.DeparturePlace == null)
                    && (trip.ArrivalPlace == parameters.ArrivalPlace || parameters.ArrivalPlace == null)));

            if (parameters is not null)
            {
                SearchByPlace(ref trips, parameters.Place);

                var sortedTrips = _sortHelper.ApplySort(trips, parameters.OrderBy);

                if (parameters.PageSize.HasValue)
                {
                    return PagedList<Trip>.ToPagedList(sortedTrips, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<Trip>.ToPagedList(sortedTrips, 1, trips.Count());
                }
            }
            else
            {
                var sortedTrips = _sortHelper.ApplySort(trips, "Date desc");
                return PagedList<Trip>.ToPagedList(sortedTrips, 1, trips.Count());
            }
        }

        public Trip GetTripById(Guid idTrip)
        {
            return FindByCondition(trip => trip.Id.Equals(idTrip))
                    .FirstOrDefault();
        }

        public void CreateTrip(Trip trip)
        {
            trip.Id = Guid.NewGuid();
            Create(trip);
            Save();
        }

        public void UpdateTrip(Trip trip)
        {
            Update(trip);
            Save();
        }

        public void DeleteTrip(Trip trip)
        {
            Delete(trip);
            Save();
        }
    }
}
