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
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private const string USER_ROLE = "USER";
        public UserRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<User> GetAllUsers()
        {
            return FindAll()
                .OrderBy(user => user.Id)
                .ToList();
        }
        public User GetUserById(int idUser)
        {
            return FindByCondition(user => user.Id.Equals(idUser))
                    .FirstOrDefault();
        }

        public User GetUserByEmail(string userEmail)
        {
            return FindByCondition(user => user.Email.Equals(userEmail))
                    .FirstOrDefault();
        }

        public void CreateUser(User user)
        {
            user.Role = USER_ROLE;
            user.RefreshToken = "";
            user.RefreshTokenExpireDate = DateTime.Now;
            Create(user);
            Save();
        }
        public void UpdateUserRole(User user)
        {
            Update(user);
            Save();
        }
        public void UpdateUserRefreshToken(User user, string refreshToken, DateTime? expireDate)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireDate = (DateTime)expireDate;
            Update(user);
            Save();
        }
    }
}
