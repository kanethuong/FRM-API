using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly DataContext _dataContext;
        public CertificateService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<int> InsertCertificate(Certificate certificate)
        {
            if (!(_dataContext.Trainees.Any(t => t.TraineeId == certificate.TraineeId)) || !(_dataContext.Modules.Any(m => m.ModuleId == certificate.ModuleId)))
            {
                return -2;
            }
            if (_dataContext.Certificates.Any(cert => cert.ModuleId == certificate.ModuleId && cert.TraineeId == certificate.TraineeId))
            {
                return -1;
            }
            _dataContext.Certificates.Add(certificate);
            return await _dataContext.SaveChangesAsync();
        }

        public async Task<string> GetCertificatesURLByTraineeIdAndModuleId(int Traineeid, int Moduleid)
        {
            var certificate = await _dataContext.Certificates.Where(m => m.TraineeId == Traineeid && m.ModuleId == Moduleid).FirstOrDefaultAsync();
            return certificate.CertificateURL;
        }


    }
}