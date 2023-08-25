using Contracts;
using Entities;
using Entities.DTOs.BeneficiaryFamily;
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
    public class BeneficiaryFamilyRepository: RepositoryBase<BeneficiaryFamily>, IBeneficiaryFamilyRepository
    {
        private readonly PersonRepository _personRepository;
        private readonly IBeneficiaryFamilyMemberRepository _member;
        private readonly ISortHelper<BeneficiaryFamily> _sortHelper;

        public BeneficiaryFamilyRepository(RepositoryContext repositoryContext, ISortHelper<BeneficiaryFamily> sortHelper, IBeneficiaryFamilyMemberRepository member)
            : base(repositoryContext)
        {
            _personRepository = new PersonRepository(repositoryContext);
            _sortHelper = sortHelper;
            _member = member;
        }
        private void SearchByName(ref IQueryable<BeneficiaryFamily> beneficiaries, string name)
        {
            if (!beneficiaries.Any() || string.IsNullOrWhiteSpace(name))
                return;

            IEnumerable<Guid> peopleIds = _personRepository.GetAllPeople().Where(person =>
                person.CI.ToString().Contains(name.Trim()) || //check if the user is searching a beneficiary by the CI
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

            beneficiaries = beneficiaries.Where(beneficiary => peopleIds.Contains(beneficiary.IDPerson));
        }

        public  PagedList<BeneficiaryFamily> GetAllBeneficiaryFamilies([Optional] BeneficiaryFamilyParameters? parameters)
        {
            IQueryable<BeneficiaryFamily> beneficiaries = FindByCondition(beneficiary =>
                (int)beneficiary.HousingType == parameters.HousingType || parameters.HousingType == null);

            if (parameters is not null)
            {
                SearchByName(ref beneficiaries, parameters.Name);
                var sortedBeneficiaries = _sortHelper.ApplySort(beneficiaries, parameters.OrderBy);

                if (parameters.PageSize.HasValue)
                {
                    return PagedList<BeneficiaryFamily>.ToPagedList(sortedBeneficiaries, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<BeneficiaryFamily>.ToPagedList(sortedBeneficiaries, 1, sortedBeneficiaries.Count());
                }
            }
            else
            {
                var sortedBeneficiaries = _sortHelper.ApplySort(beneficiaries, "RegistrationDate");
                return PagedList<BeneficiaryFamily>.ToPagedList(sortedBeneficiaries, 1, sortedBeneficiaries.Count());
            }
        }

        public BeneficiaryFamily GetBeneficiaryFamilyById(Guid idBeneficiaryFamily)
        {
            return FindByCondition(beneficiaryFamily => beneficiaryFamily.Id.Equals(idBeneficiaryFamily))
                    .FirstOrDefault();
        }
        public BeneficiaryFamily GetBeneficiaryFamilyByIdPerson(Guid idPerson)
        {
            return FindByCondition(beneficiaryFamily => beneficiaryFamily.IDPerson.Equals(idPerson))
                    .FirstOrDefault();
        }

        private static int GetAge(DateTime bday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - bday.Year;
            if (bday.AddYears(age) > now)
                age--;
            return age;
        }

        public IEnumerable<BeneficiaryFamilyMember> GetMembersByIdBeneficiary(Guid idBeneficiary)
        {
            return _member.FindAll().Where(member => member.IDBeneficiary.Equals(idBeneficiary))
                .OrderBy(member => member.Id)
                .ToList();

        }

        public BeneficiaryPopulationDTO GetPopulationByIdBeneficiary(Guid idBeneficiary)
        {
            var familyMembers = _member.FindAll().Where(member => member.IDBeneficiary.Equals(idBeneficiary)).ToList();

            var population = new BeneficiaryPopulationDTO();

            if (familyMembers is not null && familyMembers.Count > 0)
            {
                var totalPopulation = familyMembers.Count;
                var totalMembersBetween0And2Years = familyMembers.Where(member => GetAge(member.DateOfBirth) >= 0 && GetAge(member.DateOfBirth) <= 2).Count();
                var totalMembersBetween3And5Years = familyMembers.Where(member => GetAge(member.DateOfBirth) >= 3 && GetAge(member.DateOfBirth) <= 5).Count();
                var totalMembersBetween6And18Years = familyMembers.Where(member => GetAge(member.DateOfBirth) >= 6 && GetAge(member.DateOfBirth) <= 18).Count();
                var totalMembersBetween19And49Years = familyMembers.Where(member => GetAge(member.DateOfBirth) >= 19 && GetAge(member.DateOfBirth) <= 49).Count();
                var totalMembersOver50Years = familyMembers.Where(member => GetAge(member.DateOfBirth) >= 50).Count();
                var totalMembersWithDisabilities = familyMembers.Where(member => member.HasDisability).Count();

                //Set return values
                population.TotalPopulation = totalPopulation;
                population.MemberAgesBetween0And2Years = totalMembersBetween0And2Years;
                population.MemberAgesBetween3And5Years = totalMembersBetween3And5Years;
                population.MemberAgesBetween6And18Years = totalMembersBetween6And18Years;
                population.MemberAgesBetween19And49Years = totalMembersBetween19And49Years;
                population.MemberAgesOver50Years = totalMembersOver50Years;
                population.TotalMembersWithDisabilities = totalMembersWithDisabilities;
            }

            population.IDBeneficiary = idBeneficiary;
            return population;
        }

        public void CreateBeneficiaryFamily(BeneficiaryFamily beneficiaryFamily)
        {
            beneficiaryFamily.Id = Guid.NewGuid();
            Create(beneficiaryFamily);
            Save();
        }

        public void UpdateBeneficiaryFamily(BeneficiaryFamily beneficiaryFamily)
        {
            Update(beneficiaryFamily);
            Save();
        }

        public void DeleteBeneficiaryFamily(BeneficiaryFamily beneficiaryFamily)
        {
            Delete(beneficiaryFamily);
            Save();
        }
    }
}
