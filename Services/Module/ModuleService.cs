using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class ModuleService : IModuleService
    {
        private DataContext _dataContext;

        public ModuleService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Module> GetModuleById(int id)
        {
            return await _dataContext.Modules.Where(m => m.ModuleId == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertNewModule(Module module)
        {
            if (_dataContext.Modules.Any(m => m.ModuleId == module.ModuleId))
            {
                return -1;
            }
            int rowInserted = 0;
            _dataContext.Modules.Add(module);
            rowInserted = await _dataContext.SaveChangesAsync();
            return rowInserted;
        }

        public async Task<int> UpdateModule(int id, Module module)
        {
            var existedModule = await _dataContext.Modules.Where(m => m.ModuleId == id).FirstOrDefaultAsync();
            if (existedModule is null)
            {
                return -1;
            }
            existedModule.ModuleName = module.ModuleName;
            existedModule.Description = module.Description;
            existedModule.NoOfSlot = module.NoOfSlot;
            existedModule.SyllabusURL = module.SyllabusURL;
            var rowUpdated = 0;
            rowUpdated = await _dataContext.SaveChangesAsync();
            return rowUpdated;
        }


    }
}