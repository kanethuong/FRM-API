using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;

namespace kroniiapi.Services.Certificate
{
    public class CertificateService :ICertificateService
    {
        private readonly DataContext _dataContext;
        public CertificateService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
    }
}