using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Services
{
    public interface IBonusAndPunishService
    {
        Task<int> InsertNewBonusAndPunish(BonusAndPunish bonusAndPunish);
        Task<Tuple<int, IEnumerable<BonusAndPunish>>> GetBonusAndPunishList(PaginationParameter paginationParameter);
        Task<IEnumerable<BonusAndPunish>> GetBonusAndPunishListByClassId(int classId);

    }
}