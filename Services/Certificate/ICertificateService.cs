using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;

namespace kroniiapi.Services
{
    public interface ICertificateService
    {
        Task<string> GetCertificatesURLByTraineeIdAndModuleId(int Traineeid, int Moduleid);
    }
}