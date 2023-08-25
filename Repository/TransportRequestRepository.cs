using Contracts;
using Entities;
using Entities.Helpers;
using Entities.Models;
using Entities.Models.Enums;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TransportRequestRepository: RepositoryBase<TransportRequest>, ITransportRequestRepository
    {
        private readonly VolunteerRepository _volunteerRepository;
        private readonly PersonRepository _personRepository;
        private readonly ISortHelper<TransportRequest> _sortHelperTransportRequest;
        public TransportRequestRepository(RepositoryContext repositoryContext, ISortHelper<TransportRequest> sortHelperTransportRequest, ISortHelper<Volunteer> sortHelperVolunteer)
            : base(repositoryContext)
        {
            _personRepository = new PersonRepository(repositoryContext);
            _volunteerRepository = new VolunteerRepository(repositoryContext, sortHelperVolunteer);
            _sortHelperTransportRequest = sortHelperTransportRequest;
        }

        private void SearchByName(ref IQueryable<TransportRequest> transportRequests, string name)
        {
            if (!transportRequests.Any() || string.IsNullOrWhiteSpace(name))
                return;

            var peopleIds = _personRepository.GetAllPeople().Where(person =>
                person.FirstName.ToLower().Contains(name.Trim().ToLower()) ||
                person.MiddleName.ToLower().Contains(name.Trim().ToLower()) ||
                person.FirstSurname.ToLower().Contains(name.Trim().ToLower()) ||
                person.SecondSurname.ToLower().Contains(name.Trim().ToLower()) ||
                (person.FirstName + " " + person.FirstSurname).ToLower().Contains(name.ToLower()) ||
                (person.MiddleName == null
                    ? (person.FirstName + " " + person.FirstSurname)
                    : (person.SecondSurname == null 
                        ? (person.FirstName + " " + person.MiddleName + " " + person.FirstSurname)
                        : (person.FirstName + " " + person.MiddleName + " " + person.FirstSurname + " " + person.SecondSurname)
                       )
                ).ToLower().Contains(name.ToLower())
            ).Select(person => person.Id);

            var volunteerParams = new VolunteerParameters();
            var volunteers = _volunteerRepository.GetAllVolunteers(volunteerParams);

            var selectedVolunteerIds = volunteers.Where(volunteer => peopleIds.Contains(volunteer.IDPerson)).Select(volunteer => volunteer.Id).ToList();

            transportRequests = transportRequests.Where(request => selectedVolunteerIds.Contains(request.IDCoordinator));
        }

        public PagedList<TransportRequest> GetAllTransportRequests([Optional] TransportRequestParameters parameters)
        {
            var transportRequests = FindByCondition(request =>
                (request.IDDepartment == parameters.IDDepartment || parameters.IDDepartment == null) &&
                ((int)request.TransportType == parameters.TransportType || parameters.TransportType == null) &&
                ((int)request.Priority == parameters.Priority || parameters.Priority == null) &&
                ((int)request.Status == parameters.Status || parameters.Status == null) &&
                (request.Date >= parameters.MinRequestDate || parameters.MinRequestDate == null) &&
                (request.Date <= parameters.MaxRequestDate || parameters.MaxRequestDate == null)
            );

            SearchByName(ref transportRequests, parameters.Name);

            var sortedTransportRequests = _sortHelperTransportRequest.ApplySort(transportRequests, parameters.OrderBy);

            if (parameters.PageSize.HasValue)
            {
                return PagedList<TransportRequest>.ToPagedList(sortedTransportRequests, parameters.PageNumber, parameters.PageSize.Value);
            }
            else
            {
                return PagedList<TransportRequest>.ToPagedList(sortedTransportRequests, 1, sortedTransportRequests.Count());
            }
        }

        public TransportRequest GetTransportRequestById(Guid idTransportRequest)
        {
            return FindByCondition(transportRequest => transportRequest.Id.Equals(idTransportRequest))
                    .FirstOrDefault();
        }

        public void CreateTransportRequest(TransportRequest transportRequest)
        {
            transportRequest.Id = Guid.NewGuid();
            transportRequest.Status = StatusType.PENDING;
            transportRequest.CreatedAt = DateTime.Now;
            transportRequest.UpdatedAt = DateTime.Now;
            Create(transportRequest);
            Save();
        }

        public void UpdateTransportRequest(TransportRequest transportRequest)
        {
            transportRequest.UpdatedAt = DateTime.Now;
            Update(transportRequest);
            Save();
        }

        public void DeleteTransportRequest(TransportRequest transportRequest)
        {
            Delete(transportRequest);
            Save();
        }
    }
}
