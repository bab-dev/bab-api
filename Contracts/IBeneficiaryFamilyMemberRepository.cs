using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBeneficiaryFamilyMemberRepository : IRepositoryBase<BeneficiaryFamilyMember>
    {
        IEnumerable<BeneficiaryFamilyMember> GetAllBeneficiaryFamilyMembers();
        BeneficiaryFamilyMember GetBeneficiaryFamilyMemberById(Guid idMember);
        void CreateBeneficiaryFamilyMember(BeneficiaryFamilyMember member);
        void UpdateBeneficiaryFamilyMember(BeneficiaryFamilyMember member);
        void DeleteBeneficiaryFamilyMember(BeneficiaryFamilyMember member);
    }
}
