using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

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
            return null;
        }

        public async Task<int> InsertNewModule(Module module)
        {
            return 0;
        }

        public async Task<int> UpdateModule(int id, Module module)
        {
            return 0;
        }

        public async Task<int> DeleteModule(int id)
        {
            return 0;
        }
    }
}