using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BeneficiaryFamilyMemberRepository : RepositoryBase<BeneficiaryFamilyMember>, IBeneficiaryFamilyMemberRepository
    {
        public BeneficiaryFamilyMemberRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public IEnumerable<BeneficiaryFamilyMember> GetAllBeneficiaryFamilyMembers()
        {
            return FindAll()
                .OrderBy(member => member.Id)
                .ToList();
        }

        public BeneficiaryFamilyMember GetBeneficiaryFamilyMemberById(Guid idMember)
        {
            return FindByCondition(member => member.Id.Equals(idMember))
                    .FirstOrDefault();
        }

        public void CreateBeneficiaryFamilyMember(BeneficiaryFamilyMember member)
        {
            member.Id = Guid.NewGuid();
            Create(member);
            Save();
        }

        public void UpdateBeneficiaryFamilyMember(BeneficiaryFamilyMember member)
        {
            Update(member);
            Save();
        }

        public void DeleteBeneficiaryFamilyMember(BeneficiaryFamilyMember member)
        {
            Delete(member);
            Save();
        }
    }
}
