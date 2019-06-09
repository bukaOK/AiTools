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
    public class AnalyzeRepository : Repository<AnalyzeResult>
    {
        public AnalyzeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IList<AnalyzeResult>> GetByUserAsync(string userId)
        {
            return await Table.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
