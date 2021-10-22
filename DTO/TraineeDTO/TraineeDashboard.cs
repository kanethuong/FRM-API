using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.TraineeDTO
{
    public class TraineeDashboard
    {
        public IEnumerable<ModuleInTraineeDashboard> moduleInTraineeDashboards {get;set;}
        public IEnumerable<ExamInTraineeDashboard> examInTraineeDashboards {get;set;}
    }
}