using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KCSystem.Core.Interface;

namespace KCSystem.Infrastructrue.Database
{
   public class UnitOfWork:IUnitOfWork
    {
        private readonly KCDBContext _dbContext;

        public UnitOfWork(KCDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> SaveAsync()
        { 
            return await _dbContext.SaveChangesAsync() > 0; 
            
        }
    }
}
