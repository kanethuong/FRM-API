using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.MarkDTO
{
    public class MarkResponse
    {
        public string TraineeName { get; set; }
        public IEnumerable<ModuleMark> ScoreList { get; set; }
        
    }
}