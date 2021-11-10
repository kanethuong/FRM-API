using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TimetableDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace kroniiapi.Services
{
    public class ModuleService : IModuleService
    {
        private DataContext _dataContext;
        private IMegaHelper _megaHelper;

        public ModuleService(DataContext dataContext, IMegaHelper megaHelper)
        {
            _dataContext = dataContext;
            _megaHelper = megaHelper;
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
            var existedModule = await GetModuleById(id);
            if (existedModule is null)
            {
                return -1;
            }
            existedModule.ModuleName = module.ModuleName;
            existedModule.Description = module.Description;
            existedModule.NoOfSlot = module.NoOfSlot;
            existedModule.SlotDuration = module.SlotDuration;
            existedModule.SyllabusURL = module.SyllabusURL;
            existedModule.IconURL = module.IconURL;
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
        /// Get module by class id
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Module>> GetModulesByClassIdAndTrainerId(int classId, int trainerId)
        {
            List<int> moduleId = await _dataContext.ClassModules.Where(m => m.ClassId == classId && m.TrainerId == trainerId).Select(m => m.ModuleId).ToListAsync();
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
        /// Get module by trainee id
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<int>> GetModulesIdByTraineeId(int traineeId)
        {
            var classId = await _dataContext.Trainees.Where(t => t.TraineeId == traineeId).Select(t => t.ClassId).FirstOrDefaultAsync();
            var classModules = await _dataContext.ClassModules.Where(t => t.ClassId == classId).ToListAsync();
            List<int> modules = new List<int>();
            foreach (var item in classModules)
            {
                modules.Add(await _dataContext.Modules.Where(t => t.ModuleId == item.ModuleId).Select(t => t.ModuleId).FirstOrDefaultAsync());
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
            IQueryable<Module> modules = _dataContext.Modules;
            if (paginationParameter.SearchName != "")
            {
                modules = modules.Where(e => EF.Functions.ToTsVector("simple", EF.Functions.Unaccent(e.ModuleName.ToLower()))
                    .Matches(EF.Functions.ToTsQuery("simple", EF.Functions.Unaccent(paginationParameter.SearchName.ToLower()))));
            }
            IEnumerable<Module> rs = await modules
                .GetCount(out var totalRecords)
                .OrderByDescending(e => e.CreatedAt)
                .GetPage(paginationParameter)
                .ToListAsync();
            return Tuple.Create(totalRecords, rs);
        }

        /// <summary>
        /// Get modules by trainee id
        /// </summary>
        /// <param name="traineeId"></param>
        /// <returns>Modules</returns>
        public async Task<IEnumerable<Module>> GetModulesByTraineeId(int traineeId)
        {
            var classId = await _dataContext.Trainees.Where(t => t.TraineeId == traineeId).Select(t => t.ClassId).FirstOrDefaultAsync();
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
        public async Task<IEnumerable<ModuleExcelInput>> GetModuleLessonDetail(int moduleId)
        {
            var SyllabusURL = await _dataContext.Modules.Where(m => m.ModuleId == moduleId).Select(m => m.SyllabusURL).FirstOrDefaultAsync();
            if (SyllabusURL == null)
            {
                return null;
            }
            var stream = await _megaHelper.Download(new Uri(SyllabusURL));

            List<ModuleExcelInput> moduleExcelInputs = new();
            using var package = new ExcelPackage(stream);

            ExcelWorkbook workbook = package.Workbook;
            if (workbook.Worksheets.Count() < 2)
            {
                return null;
            }
            ExcelWorksheet syllabusSheet = workbook.Worksheets[1];

            if (syllabusSheet == null)
            {
                return null;
            }
            var range = syllabusSheet.Cells[3, 2, syllabusSheet.Dimension.Rows, 7];
            var days = new HashSet<int>();
            string lecture = "";

            range.ExportDataFromCells(cells =>
            {
                var content = cells[2].Value;
                if (content == null)
                {
                    return;
                }
                var deliveryType = cells[4].Value.ToString();
                var duration = int.Parse(cells[5].Value.ToString());
                var lectureCell = cells[1].Value;
                if (lectureCell != null)
                {
                    lecture = lectureCell.ToString();
                }
                var dayCell = cells[0].Value;

                if (dayCell != null)
                {
                    days = new HashSet<int>();

                    var dayString = dayCell.ToString();
                    var split = dayString.Split(",");
                    foreach (var item in split)
                    {
                        if (int.TryParse(item.Trim(), out var day))
                        {
                            days.Add(day);
                        }
                    }
                }

                foreach (var day in days)
                {
                    ModuleExcelInput moduleExcelInput = new();
                    moduleExcelInput.Day = day;
                    moduleExcelInput.Lecture = lecture;
                    moduleExcelInput.DeliveryType = deliveryType;
                    moduleExcelInput.Duration_mins = duration;
                    moduleExcelInputs.Add(moduleExcelInput);
                }

            });
            var resultAll = moduleExcelInputs.OrderBy(c => c.Day);
            return resultAll;
        }
    }
}