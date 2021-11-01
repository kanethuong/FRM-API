using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface ICostService
    {
        Task<int> InsertNewCost(Cost cost);
        Task<IEnumerable<CostType>> GetCostTypeList();
    }
}