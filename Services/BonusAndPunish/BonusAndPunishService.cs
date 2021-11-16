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
    public class BonusAndPunishService : IBonusAndPunishService
    {
        private DataContext _dataContext;
        public BonusAndPunishService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<int> InsertNewBonusAndPunish(BonusAndPunish bonusAndPunish)
        {
            int rowInserted = 0;

            _dataContext.BonusAndPunishes.Add(bonusAndPunish);
            rowInserted = await _dataContext.SaveChangesAsync();

            return rowInserted;
        }
        public async Task<Tuple<int, IEnumerable<BonusAndPunish>>> GetBonusAndPunishList(PaginationParameter paginationParameter)
        {
            IQueryable<BonusAndPunish> bNpList = _dataContext.BonusAndPunishes.Select(b => new BonusAndPunish
            {
                BonusAndPunishId = b.BonusAndPunishId,
                CreatedAt = b.CreatedAt,
                Reason = b.Reason,
                Score = b.Score,
                TraineeId = b.TraineeId,
                Trainee = new Trainee
                {
                    TraineeId = b.Trainee.TraineeId,
                    Fullname = b.Trainee.Fullname,
                    AvatarURL = b.Trainee.AvatarURL,
                    ClassId = b.Trainee.ClassId,
                    Email = b.Trainee.Email,
                    Class = new Class
                    {
                        ClassName = b.Trainee.Class.ClassName
                    }
                }
            });
            if (paginationParameter.SearchName != "")
            {
                bNpList = bNpList.Where(c => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(c.Reason.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Trainee.Fullname.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Trainee.Email.ToLower())
                                                                                        + " "
                                                                                        + EF.Functions.Unaccent(c.Trainee.Class.ClassName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }

            IEnumerable<BonusAndPunish> rs = await bNpList
                .GetCount(out var totalRecords)
                .OrderBy(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();

            return Tuple.Create(totalRecords, rs);
        }
        
    }
}