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
    public class BeneficiaryOrganizationRepository : RepositoryBase<BeneficiaryOrganization>, IBeneficiaryOrganizationRepository
    {
        private readonly ISortHelper<BeneficiaryOrganization> _sortHelper;
        public BeneficiaryOrganizationRepository(RepositoryContext repositoryContext, ISortHelper<BeneficiaryOrganization> sortHelper)
            : base(repositoryContext)
        {
            _sortHelper = sortHelper;
        }
        private void SearchByOrganizationName(ref IQueryable<BeneficiaryOrganization> organizations, string organizationName)
        {
            if (!organizations.Any() || string.IsNullOrWhiteSpace(organizationName))
                return;

            organizations = organizations.Where(organization =>
                organization.OrganizationName.ToLower().Contains(organizationName.Trim().ToLower()));
        }

        public PagedList<BeneficiaryOrganization> GetAllBeneficiaryOrganizations([Optional] BeneficiaryOrganizationParameters parameters)
        {
            IQueryable<BeneficiaryOrganization> organizations = FindByCondition(organization =>
                ((int)organization.OrganizationType == parameters.OrganizationType || parameters.OrganizationType == null) 
                    && (organization.IDCoordinator == parameters.IDCoordinator || parameters.IDCoordinator == null));

            if (parameters is not null)
            {
                SearchByOrganizationName(ref organizations, parameters.OrganizationName);

                var sortedOrganizations = _sortHelper.ApplySort(organizations, parameters.OrderBy);

                if (parameters.PageSize.HasValue)
                {
                    return PagedList<BeneficiaryOrganization>.ToPagedList(sortedOrganizations, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<BeneficiaryOrganization>.ToPagedList(sortedOrganizations, 1, organizations.Count());
                }
            }
            else
            {
                var sortedOrganizations = _sortHelper.ApplySort(organizations, "OrganizationName");
                return PagedList<BeneficiaryOrganization>.ToPagedList(sortedOrganizations, 1, organizations.Count());
            }
        }

        public BeneficiaryOrganization GetBeneficiaryOrganizationById(Guid idBeneficiaryOrganization)
        {
            return FindByCondition(BeneficiaryOrganization => BeneficiaryOrganization.Id.Equals(idBeneficiaryOrganization))
                    .FirstOrDefault();
        }

        public void CreateBeneficiaryOrganization(BeneficiaryOrganization BeneficiaryOrganization)
        {
            BeneficiaryOrganization.Id = Guid.NewGuid();
            Create(BeneficiaryOrganization);
            Save();
        }

        public void UpdateBeneficiaryOrganization(BeneficiaryOrganization BeneficiaryOrganization)
        {
            Update(BeneficiaryOrganization);
            Save();
        }

        public void DeleteBeneficiaryOrganization(BeneficiaryOrganization BeneficiaryOrganization)
        {
            Delete(BeneficiaryOrganization);
            Save();
        }
    }
}
