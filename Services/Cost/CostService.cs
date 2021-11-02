using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class CostService : ICostService
    {
        private DataContext _dataContext;
        public CostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<int> InsertNewCost(Cost cost)
        {
            var checkType = _dataContext.CostTypes.Any(t => t.CostTypeId == cost.CostTypeId);
            if (checkType is false)
            {
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

        public async Task<Tuple<int, IEnumerable<Cost>>> GetCostList(PaginationParameter paginationParameter)
        {
            var costList = await _dataContext.Costs.Where(c => c.Content.ToLower().Contains(paginationParameter.SearchName.ToLower()))
                                .Select(c => new Cost
                                {
                                    CostId = c.CostId,
                                    Content = c.Content,
                                    Amount = c.Amount,
                                    CreatedAt = c.CreatedAt,
                                    CostType = new CostType
                                    {
                                        CostTypeName = c.CostType.CostTypeName
                                    },
                                    Admin = new Admin
                                    {
                                        Fullname = c.Admin.Fullname
                                    }
                                })
                                .OrderByDescending(c => c.CreatedAt)
                                .ToListAsync();
            return Tuple.Create(costList.Count(), PaginationHelper.GetPage(costList, paginationParameter));
        }
    }
}