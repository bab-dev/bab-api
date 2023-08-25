using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDepartmentRepository : IRepositoryBase<Department> 
    {
        IEnumerable<Department> GetAllDepartments();
        Department GetDepartmentById(Guid idDepartment);
        void CreateDepartment(Department department);
        void UpdateDepartment(Department department);
        void DeleteDepartment(Department department);
    }
}
