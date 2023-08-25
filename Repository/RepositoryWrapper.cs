using Contracts;
using Entities;
using Entities.DTOs.Clocking;
using Entities.Helpers;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private readonly IConfiguration _config;
        private IUserRepository _user;
        private IPersonRepository _person;
        private IVolunteerRepository _volunteer;
        private IDepartmentRepository _department;
        private IEmergencyContactRepository _emergencyContact;
        private IVolunteerAvailabilityRepository _volunteerAvailability;
        private ITransportRequestRepository _transportRequest;
        private ICompanyRepository _company;
        private IMarketSellerRepository _marketSeller;
        private IProductCategoryRepository _productCategory;
        private IEventRepository _event;
        private IEventVolunteerRepository _eventVolunteer;
        private IBeneficiaryFamilyRepository _beneficiaryFamily;
        private IBeneficiaryFamilyMemberRepository _beneficiaryFamilyMember;
        private IBeneficiaryOrganizationRepository _beneficiaryOrganization;
        private ITripRepository _trip;
        private IMarketRepository _market;
        private IClockingRepository _clocking;
        private IVolunteerEventWorkedOnRepository _eventWorkedOn;
        private IJWTManagerRepository _jWTManager;

        // Sort Helpers
        private readonly ISortHelper<TransportRequest> _transportRequestSortHelper;
        private readonly ISortHelper<Volunteer> _volunteerSortHelper;
        private readonly ISortHelper<Company> _companySortHelper;
        private readonly ISortHelper<MarketSeller> _marketSellerSortHelper;
        private readonly ISortHelper<Event> _eventSortHelper;
        private readonly ISortHelper<BeneficiaryFamily> _beneficiaryFamilySortHelper;
        private readonly ISortHelper<BeneficiaryOrganization> _beneficiaryOrganizationSortHelper;
        private readonly ISortHelper<Trip> _tripSortHelper;
        private readonly ISortHelper<Clocking> _clockingSortHelper;
        private readonly ISortHelper<VolunteerWorkStatisticsDTO> _volunteerWorkStatisticsSortHelper;

        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_repoContext);
                }
                return _user;
            }
        }
        public IPersonRepository Person
        {
            get
            {
                if (_person == null)
                {
                    _person = new PersonRepository(_repoContext);
                }
                return _person;
            }
        }
        public IVolunteerRepository Volunteer
        {
            get
            {
                if (_volunteer == null)
                {
                    _volunteer = new VolunteerRepository(_repoContext, _volunteerSortHelper);
                }
                return _volunteer;
            }
        }
        public IDepartmentRepository Department
        {
            get
            {
                if (_department == null)
                {
                    _department = new DepartmentRepository(_repoContext);
                }
                return _department;
            }
        }
        public IEmergencyContactRepository EmergencyContact
        {
            get
            {
                if (_emergencyContact == null)
                {
                    _emergencyContact = new EmergencyContactRepository(_repoContext);
                }
                return _emergencyContact;
            }
        }
        public IVolunteerAvailabilityRepository VolunteerAvailability
        {
            get
            {
                if (_volunteerAvailability == null)
                {
                    _volunteerAvailability = new VolunteerAvailabilityRepository(_repoContext);
                }
                return _volunteerAvailability;
            }
        }
        public IJWTManagerRepository JWTManager
        {
            get
            {
                if (_jWTManager == null)
                {
                    _jWTManager = new JWTManagerRepository(_repoContext, _config);
                }
                return _jWTManager;
            }
        }
        public ITransportRequestRepository TransportRequest
        {
            get
            {
                if (_transportRequest == null)
                {
                    _transportRequest = new TransportRequestRepository(_repoContext, _transportRequestSortHelper, _volunteerSortHelper);
                }
                return _transportRequest;
            }
        }
        public ICompanyRepository Company
        {
            get
            {
                if (_company == null)
                {
                    _company = new CompanyRepository(_repoContext, _companySortHelper);
                }
                return _company;
            }
        }
        public IProductCategoryRepository ProductCategory
        {
            get
            {
                if (_productCategory == null)
                {
                    _productCategory = new ProductCategoryRepository(_repoContext);
                }
                return _productCategory;
            }
        }

        public IMarketSellerRepository MarketSeller
        {
            get
            {
                if (_marketSeller == null)
                {
                    _marketSeller = new MarketSellerRepository(_repoContext, _marketSellerSortHelper);
                }
                return _marketSeller;
            }
        }

        public IEventRepository Event
        {
            get
            {
                if (_event == null)
                {
                    _event = new EventRepository(_repoContext, _eventSortHelper);
                }
                return _event;
            }
        }

        public IEventVolunteerRepository EventVolunteer
        {
            get
            {
                if (_eventVolunteer == null)
                {
                    _eventVolunteer = new EventVolunteerRepository(_repoContext, _event, _volunteer, _eventSortHelper);
                }
                return _eventVolunteer;
            }
        }

        public IBeneficiaryFamilyRepository BeneficiaryFamily
        {
            get
            {
                if (_beneficiaryFamily == null)
                {
                    _beneficiaryFamily = new BeneficiaryFamilyRepository(_repoContext, _beneficiaryFamilySortHelper, _beneficiaryFamilyMember);
                }
                return _beneficiaryFamily;
            }
        }
        public IBeneficiaryFamilyMemberRepository BeneficiaryFamilyMember
        {
            get
            {
                if (_beneficiaryFamilyMember == null)
                {
                    _beneficiaryFamilyMember = new BeneficiaryFamilyMemberRepository(_repoContext);
                }
                return _beneficiaryFamilyMember;
            }
        }
        public IBeneficiaryOrganizationRepository BeneficiaryOrganization
        {
            get
            {
                if (_beneficiaryOrganization == null)
                {
                    _beneficiaryOrganization = new BeneficiaryOrganizationRepository(_repoContext, _beneficiaryOrganizationSortHelper);
                }
                return _beneficiaryOrganization;
            }
        }

        public ITripRepository Trip
        {
            get
            {
                if (_trip == null)
                {
                    _trip = new TripRepository(_repoContext, _tripSortHelper);
                }
                return _trip;
            }
        }

        public IClockingRepository Clocking
        {
            get
            {
                if (_clocking == null)
                {
                    _clocking = new ClockingRepository(_repoContext, _clockingSortHelper);
                }
                return _clocking;
            }
        }

        public IMarketRepository Market
        {
            get
            {
                if (_market == null)
                {
                    _market = new MarketRepository(_repoContext);
                }
                return _market;
            }
        }

        public IVolunteerEventWorkedOnRepository VolunteerEventWorkedOn
        {
            get
            {
                if (_eventWorkedOn == null)
                {
                    _eventWorkedOn = new VolunteerEventWorkedOnRepository(_repoContext, _volunteerWorkStatisticsSortHelper,_clocking);
                }
                return _eventWorkedOn;
            }
        }

        public RepositoryWrapper(RepositoryContext repositoryContext,
            ISortHelper<TransportRequest> transportRequestSortHelper, 
            ISortHelper<Volunteer> volunteerSortHelper, 
            ISortHelper<Company> companySortHelper,
            ISortHelper<MarketSeller> marketSellerSortHelper,
            ISortHelper<Event> eventSortHelper,
            ISortHelper<BeneficiaryFamily> beneficiaryFamilySortHelper,
            ISortHelper<BeneficiaryOrganization> beneficiaryOrganizationSortHelper,
            ISortHelper<Trip> tripSortHelper,
            ISortHelper<Clocking> clockingSortHelper,
            ISortHelper<VolunteerWorkStatisticsDTO> volunteerWorkStatisticsSortHelper,
            IConfiguration config,
            IEventRepository eventRepository,
            IVolunteerRepository volunteerRepository,
            IBeneficiaryFamilyMemberRepository member, IClockingRepository clocking)
        {
            _repoContext = repositoryContext;
            _config = config;
            _transportRequestSortHelper = transportRequestSortHelper;
            _volunteerSortHelper = volunteerSortHelper;
            _companySortHelper = companySortHelper;
            _marketSellerSortHelper = marketSellerSortHelper;
            _eventSortHelper = eventSortHelper;
            _beneficiaryFamilySortHelper = beneficiaryFamilySortHelper;
            _beneficiaryOrganizationSortHelper = beneficiaryOrganizationSortHelper;
            _tripSortHelper = tripSortHelper;
            _clockingSortHelper = clockingSortHelper;
            _volunteerWorkStatisticsSortHelper = volunteerWorkStatisticsSortHelper;

            _event = eventRepository;
            _volunteer = volunteerRepository;
            _beneficiaryFamilyMember = member;
            _clocking = clocking;
        }

        public void Save()
        {
            _repoContext.SaveChanges();
        }
    }
}
