using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IBonusAndPunishService
    {
        Task<int> InsertNewBonusAndPunish(BonusAndPunish bonusAndPunish);
    }
}