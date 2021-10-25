using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO.TraineeDTO;

namespace kroniiapi.Services
{
    public interface ICertificateService
    {
        Task<int> InsertCertificate(Certificate certificate);
    }
}