using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;

namespace kroniiapi.Services
{
    public class BonusAndPunishService : IBonusAndPunishService
    {
        private DataContext _dataContext;
        public BonusAndPunishService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<int> InsertNewCost(BonusAndPunish bonusAndPunish)
        {
            return 0;
        }
        public async Task<Tuple<int, IEnumerable<BonusAndPunish>>> GetBonusAndPunishList(PaginationParameter paginationParameter)
        {
            return null;
        }
    }
}