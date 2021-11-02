using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class CostService :ICostService
    {
        private DataContext _dataContext;
        public CostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<int> InsertNewCost(Cost cost)
        {
            var checkType =_dataContext.CostTypes.Any(t => t.CostTypeId == cost.CostTypeId);
            if(checkType is false){
                return -1;
            }
            int rowInserted = 0;
            _dataContext.Costs.Add(cost);
            rowInserted = await _dataContext.SaveChangesAsync();
            return rowInserted;
        }
        public async Task<IEnumerable<CostType>> GetCostTypeList()
        {
            return await _dataContext.CostTypes.ToListAsync();
        }
    }
}