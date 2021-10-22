using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class EventInTimeTable
    {
        public IEnumerable<ModuleInTimeTable> moduleInTimeTables{get;set;}
        public IEnumerable<ExamInTimeTable> examInTimeTables { get; set; }
    }
}