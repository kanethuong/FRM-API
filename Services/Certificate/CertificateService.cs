using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapi.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly DataContext _dataContext;
        public CertificateService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<int> InsertCertificate(Certificate certificate) {
            if (_dataContext.Certificates.Any(cert => cert.ModuleId == certificate.ModuleId && cert.TraineeId == certificate.TraineeId)){
                return 0;
            }
            _dataContext.Certificates.Add(certificate);
            return await _dataContext.SaveChangesAsync();
        }
    }
}