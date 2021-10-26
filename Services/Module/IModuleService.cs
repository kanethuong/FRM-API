using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface IModuleService
    {
        Task<Module> GetModuleById(int id);
        Task<int> InsertNewModule(Module module);
        Task<int> UpdateModule(int id, Module module);
        Task<IEnumerable<Module>> GetModulesByClassId(int classId);
    }
}