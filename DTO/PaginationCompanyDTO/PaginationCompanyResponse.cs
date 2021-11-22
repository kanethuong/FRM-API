using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.PaginationCompanyDTO
{
    public class PaginationCompanyResponse<T>
    {
        public int TotalRecords { get; set; } = 0;
        public T Payload { get; set; }

        public PaginationCompanyResponse() { }

        public PaginationCompanyResponse(int totalRecords, T payload)
        {
            this.TotalRecords = totalRecords;
            this.Payload = payload;
        }
    }
}