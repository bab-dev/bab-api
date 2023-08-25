using AutoMapper;
using Entities.DTOs;
using Entities.DTOs.BeneficiaryFamily;
using Entities.DTOs.BeneficiaryFamilyMember;
using Entities.DTOs.BeneficiaryOrganization;
using Entities.DTOs.Clocking;
using Entities.DTOs.NewPersonBeneficiaryFamily;
using Entities.DTOs.Trip;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BABWebApp
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserForCreationAndLoginDTO, User>();

            CreateMap<Person, PersonDTO>();
            CreateMap<Person, VolunteerPersonDTO>();
            CreateMap<PersonForCreationAndUpdateDTO, Person>();

            CreateMap<Volunteer, VolunteerDTO>();
            CreateMap<Volunteer, VolunteerPersonDTO>();
            CreateMap<VolunteerForCreationAndUpdateDTO, Volunteer>();
            CreateMap<NewVolunteerForCreationDTO, Volunteer>();
            CreateMap<Volunteer, VolunteerFromEventDTO>();

            CreateMap<Department, DepartmentDTO>();
            CreateMap<DepartmentForCreationDTO, Department>();
            CreateMap<DepartmentForUpdateDTO, Department>();

            CreateMap<EmergencyContact, EmergencyContactDTO>();
            CreateMap<EmergencyContactForCreationDTO, EmergencyContact>();
            CreateMap<NewEmergencyContactForCreation, EmergencyContact>();
            CreateMap<EmergencyContactForUpdateDTO, EmergencyContact>();

            CreateMap<VolunteerAvailability, VolunteerAvailabilityDTO>();
            CreateMap<VolunteerAvailabilityForCreationDTO, VolunteerAvailability>();
            CreateMap<NewVolunteerAvailabilityForCreationDTO, VolunteerAvailability>();
            CreateMap<VolunteerAvailabilityForUpdateDTO, VolunteerAvailability>();

            CreateMap<TransportRequest, TransportRequestDTO>();
            CreateMap<TransportRequestForCreationAndUpdateDTO, TransportRequest>();

            CreateMap<CompanyForCreationAndUpdateDTO, Company>();

            CreateMap<ProductCategory, ProductCategoryForCreationAndUpdateDTO>();
            CreateMap<ProductCategoryForCreationAndUpdateDTO, ProductCategory>();

            CreateMap<MarketSeller, MarketSellerDTO>();
            CreateMap<MarketSellerForCreationAndUpdateDTO, MarketSeller>();

            CreateMap<Event, EventDTO>();
            CreateMap<Event, EventForCreationAndUpdateDTO>();
            CreateMap<EventForCreationAndUpdateDTO, Event>();

            CreateMap<EventVolunteer, EventVolunteerForCreationAndUpdateDTO>();
            CreateMap<EventVolunteerForCreationAndUpdateDTO, EventVolunteer>();

            CreateMap<BeneficiaryFamily, BeneficiaryFamilyDTO>();
            CreateMap<BeneficiaryFamily, BeneficiaryFamilyWithPopulation>();
            CreateMap<BeneficiaryFamily, BeneficiaryFamilyForCreationAndUpdateDTO>();
            CreateMap<BeneficiaryFamilyForCreationAndUpdateDTO, BeneficiaryFamily>();
            CreateMap<NewBeneficiaryFamilyForCreationDTO, BeneficiaryFamily>();

            CreateMap<Person, BeneficiaryFamilyWithPopulation>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<BeneficiaryPopulationDTO, BeneficiaryFamilyWithPopulation>();

            CreateMap<BeneficiaryFamilyMember, BeneficiaryFamilyMemberDTO>();
            CreateMap<BeneficiaryFamilyMember, BeneficiaryFamilyMemberForCreationDTO>();
            CreateMap<BeneficiaryFamilyMember, BeneficiaryFamilyMemberForUpdateDTO>();
            CreateMap<BeneficiaryFamilyMemberForCreationDTO, BeneficiaryFamilyMember>();
            CreateMap<BeneficiaryFamilyMemberForUpdateDTO, BeneficiaryFamilyMember>();
            CreateMap<NewBeneficiaryMemberForCreationDTO, BeneficiaryFamilyMember>();

            CreateMap<BeneficiaryOrganization, BeneficiaryOrganizationDTO>();
            CreateMap<BeneficiaryOrganization, BeneficiaryOrganizationForCreationAndUpdateDTO>();
            CreateMap<BeneficiaryOrganizationForCreationAndUpdateDTO, BeneficiaryOrganization>();

            CreateMap<Trip, TripDTO>();
            CreateMap<Trip, TripForCreationAndUpdateDTO>();
            CreateMap<TripForCreationAndUpdateDTO, Trip>();

            CreateMap<Market, MarketForCreationAndUpdateDTO>();
            CreateMap<MarketForCreationAndUpdateDTO, Market>();

            CreateMap<Clocking, ClockingDTO>();
            CreateMap<Clocking, ClockingForCreationDTO>();
            CreateMap<ClockingForCreationDTO, Clocking>();
            CreateMap<Clocking, ClockingForUpdateDTO>();
            CreateMap<ClockingForUpdateDTO, Clocking>();
            CreateMap<Clocking, ClockoutDTO>();
            CreateMap<ClockoutDTO, Clocking>();
            CreateMap<ClockingDTO, Clocking>();
        }
    }
}
