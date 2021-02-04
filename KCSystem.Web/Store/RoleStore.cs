using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KCSystem.Core.Entities; 
using KCSystem.Core.Pages;
using KCSystem.Infrastructrue.Database;
using KCSystem.Infrastructrue.Extensions;
using KCSystem.Infrastructrue.Repository; 

namespace KCSystem.Web.Store
{
   public class RoleStoreImp : EfRepository<Role>, IRoleStore<Role>
    {
        


        public RoleStoreImp(KCDBContext context) : base(context)
        {
            
        }

        public async Task<PaginatedList<Role>> GetPagesAsync(QueryParameters parameters)
        {
           
           var query = _dbSet.AsQueryable();
               query = query.ApplySort(parameters.Order);

            var count = await query.CountAsync();
            var data = await query
                .Skip(parameters.PageIndex * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();
            return new PaginatedList<Role>(parameters.PageIndex, parameters.PageSize, count, data);
        }

        
        public void Dispose()
        {
             
        }

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

         
    }
}
