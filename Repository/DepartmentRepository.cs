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
    public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {
        public DepartmentRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public IEnumerable<Department> GetAllDepartments()
        {
            return FindAll()
                .OrderBy(department => department.Id)
                .ToList();
        }

        public Department GetDepartmentById(Guid idDepartment)
        {
            return FindByCondition(department => department.Id.Equals(idDepartment))
                    .FirstOrDefault();
        }

        public void CreateDepartment(Department department)
        {
            department.Id = Guid.NewGuid();
            Create(department);
            Save();
        }

        public void UpdateDepartment(Department department)
        {
            Update(department);
            Save();
        }

        public void DeleteDepartment(Department department)
        {
            Delete(department);
            Save();
        }
    }
}
