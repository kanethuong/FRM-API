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
    public class ModuleService : IModuleService
    {
        private DataContext _dataContext;

        public ModuleService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Get module by id
        /// </summary>
        /// <param name="id">module id</param>
        /// <returns>module</returns>
        public async Task<Module> GetModuleById(int id)
        {
            return await _dataContext.Modules.Where(m => m.ModuleId == id).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Insert new module to db
        /// </summary>
        /// <param name="module">new module</param>
        /// <returns>-1: duplicate / 1: successfully inserted / 0:some error</returns>
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
        /// <summary>
        /// update module
        /// </summary>
        /// <param name="id"></param>
        /// <param name="module"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Get module by class id
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Module>> GetModulesByClassId(int classId)
        {
            List<int> moduleId = await _dataContext.ClassModules.Where(m => m.ClassId == classId).Select(m => m.ModuleId).ToListAsync();
            List<Module> modules = new List<Module>();
            foreach (int i in moduleId)
            {
                Module m = new Module();
                m = await GetModuleById(i);
                modules.Add(m);
            }
            return modules;
        }
        /// <summary>
        /// Get all module with pagination (module name)
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        public async Task<Tuple<int, IEnumerable<Module>>> GetAllModule(PaginationParameter paginationParameter)
        {
            var moduleList = await _dataContext.Modules.Where(m=> m.ModuleName.ToUpper().Contains(paginationParameter.SearchName.ToUpper()))
                                                        .OrderByDescending(m=>m.CreatedAt)
                                                        .ToListAsync();
            return Tuple.Create(moduleList.Count(), PaginationHelper.GetPage(moduleList,
                paginationParameter.PageSize, paginationParameter.PageNumber));
        }
    }
}