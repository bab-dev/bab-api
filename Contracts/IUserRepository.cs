using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        IEnumerable<User> GetAllUsers();
        User GetUserById(int userId);
        User GetUserByEmail(string userEmail);
        void CreateUser(User user);
        void UpdateUserRole(User user);
        void UpdateUserRefreshToken(User user, string refreshToken, DateTime? expireDate);
    }
}
