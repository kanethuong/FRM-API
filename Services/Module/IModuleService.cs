using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TimetableDTO;

namespace kroniiapi.Services
{
    public interface IModuleService
    {
        Task<Module> GetModuleById(int id);
        Task<int> InsertNewModule(Module module);
        Task<int> UpdateModule(int id, Module module);
        Task<IEnumerable<Module>> GetModulesByClassId(int classId);
        Task<IEnumerable<Module>> GetModulesByClassIdAndTrainerId(int classId, int trainerId);
        Task<Tuple<int, IEnumerable<Module>>> GetAllModule(PaginationParameter paginationParameter);
        Task<IEnumerable<int>> GetModulesIdByTraineeId(int traineeId);
        Task<IEnumerable<Module>> GetModulesByTraineeId(int traineeId);
        Task<IEnumerable<ModuleExcelInput>> GetModuleLessonDetail(int moduleId);
        IEnumerable<ModuleExcelInput> GetModuleLessonDetail(string moduleName, Stream stream);
    }
}