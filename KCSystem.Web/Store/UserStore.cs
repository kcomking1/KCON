using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Infrastructrue.Database;
using KCSystem.Infrastructrue.Repository;

namespace KCSystem.Web.Store
{
    public  class UserStoreImp : EfRepository<User>, IUserPasswordStore<User>
    {
        public UserStoreImp(KCDBContext dbContext) : base(dbContext)
        {
        }

        public void Dispose()
        {
            
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            Update(user);

            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName.ToLower());
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await InsertAsync(user);
            
            return await Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            Update(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            Remove(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = GetByKey(int.Parse(userId));
            return Task.FromResult(user);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = QueryAll().FirstOrDefault(u => (u.Disabled == false && 
                                                                                                   u.UserName.ToUpper() == normalizedUserName.ToUpper()) 
                                                                                                   ||(u.Disabled ==true && u.UserName.ToUpper() == normalizedUserName.ToUpper() && u.UserName=="Admin"));
            return Task.FromResult(user);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.UserPassword = passwordHash;
            Update(user);

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserPassword);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.UserPassword));
        }
    }
}
