using AiTools.DAL.Core;
using AiTools.DAL.Entities;
using AiTools.DAL.Infrastucture;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiTools.DAL.Repositories
{
    public class ForecastRepository : Repository<ForecastResult>
    {
        public ForecastRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IList<ForecastResult>> GetByUserAsync(string userId)
        {
            return await Table.AsNoTracking().Where(x => x.UserId == userId)
                .ToListAsync();
        }
    }
}
