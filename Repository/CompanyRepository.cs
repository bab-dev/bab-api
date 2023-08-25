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
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        private readonly ISortHelper<Company> _sortHelper;
        public CompanyRepository(RepositoryContext repositoryContext, ISortHelper<Company> sortHelper)
            : base(repositoryContext)
        {
            _sortHelper = sortHelper;
        }
        private void SearchByName(ref IQueryable<Company> companies, string companyName)
        {
            if (!companies.Any() || string.IsNullOrWhiteSpace(companyName))
                return;

            companies = companies.Where(company => company.CompanyComercialName.Contains(companyName.Trim().ToLower()));
        }

        public PagedList<Company> GetAllCompanies([Optional] CompanyParameters parameters)
        {
            var companies = FindAll();

            if(parameters is not null)
            {
                SearchByName(ref companies, parameters.CompanyComercialName);
                
                var sortedCompanies = _sortHelper.ApplySort(companies, parameters.OrderBy);
                if (parameters.PageSize.HasValue)
                {
                    return PagedList<Company>.ToPagedList(sortedCompanies, parameters.PageNumber, parameters.PageSize.Value);
                }
                else
                {
                    return PagedList<Company>.ToPagedList(sortedCompanies, 1, sortedCompanies.Count());
                }
            } else
            {
                var sortedVolunteers = _sortHelper.ApplySort(companies, "BusinessName");
                return PagedList<Company>.ToPagedList(sortedVolunteers, 1, 10);
            }
        }

        public Company GetCompanyById(Guid idCompany)
        {
            return FindByCondition(company => company.Id.Equals(idCompany))
                    .FirstOrDefault();
        }

        public void CreateCompany(Company company)
        {
            company.Id = Guid.NewGuid();
            Create(company);
            Save();
        }

        public void UpdateCompany(Company company)
        {
            Update(company);
            Save();
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
            Save();
        }
    }
}
