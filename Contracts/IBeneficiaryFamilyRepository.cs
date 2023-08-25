using Entities.DTOs.BeneficiaryFamily;
using Entities.Models;
using Entities.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBeneficiaryFamilyRepository : IRepositoryBase<BeneficiaryFamily>
    {
        PagedList<BeneficiaryFamily> GetAllBeneficiaryFamilies(BeneficiaryFamilyParameters? parameters);
        BeneficiaryFamily GetBeneficiaryFamilyById(Guid idBeneficiaryFamily);
        BeneficiaryFamily GetBeneficiaryFamilyByIdPerson(Guid idPerson);
        IEnumerable<BeneficiaryFamilyMember> GetMembersByIdBeneficiary(Guid idBeneficiary);
        BeneficiaryPopulationDTO GetPopulationByIdBeneficiary(Guid idBeneficiary);
        void CreateBeneficiaryFamily(BeneficiaryFamily beneficiaryFamily);
        void UpdateBeneficiaryFamily(BeneficiaryFamily beneficiaryFamily);
        void DeleteBeneficiaryFamily(BeneficiaryFamily beneficiaryFamily);
    }
}
