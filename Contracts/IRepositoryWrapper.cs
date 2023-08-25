using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        IPersonRepository Person { get; }
        IVolunteerRepository Volunteer { get; }
        IDepartmentRepository Department { get; }
        IEmergencyContactRepository EmergencyContact { get; }
        IVolunteerAvailabilityRepository VolunteerAvailability { get; }
        IJWTManagerRepository JWTManager { get; }
        ITransportRequestRepository TransportRequest { get; }
        ICompanyRepository Company { get; }
        IProductCategoryRepository ProductCategory { get; }
        IMarketSellerRepository MarketSeller { get; }
        IEventRepository Event { get; }
        IEventVolunteerRepository EventVolunteer { get; }
        IBeneficiaryFamilyRepository BeneficiaryFamily { get; }
        IBeneficiaryFamilyMemberRepository BeneficiaryFamilyMember { get; }
        IBeneficiaryOrganizationRepository BeneficiaryOrganization { get; }
        ITripRepository Trip { get; }
        IMarketRepository Market { get; }
         IClockingRepository Clocking { get; }
        IVolunteerEventWorkedOnRepository VolunteerEventWorkedOn { get; }
        void Save();
    }
}
