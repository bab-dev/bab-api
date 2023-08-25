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
    public interface IBeneficiaryOrganizationRepository : IRepositoryBase<BeneficiaryOrganization>
    {
        PagedList<BeneficiaryOrganization> GetAllBeneficiaryOrganizations([Optional] BeneficiaryOrganizationParameters parameters);
        BeneficiaryOrganization GetBeneficiaryOrganizationById(Guid idBeneficiaryOrganization);
        void CreateBeneficiaryOrganization(BeneficiaryOrganization beneficiaryOrganization);
        void UpdateBeneficiaryOrganization(BeneficiaryOrganization beneficiaryOrganization);
        void DeleteBeneficiaryOrganization(BeneficiaryOrganization beneficiaryOrganization);
    }
}
