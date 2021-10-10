using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.PaginationDTO
{
    public class PaginationResponse<T>
    {
        public int TotalRecords { get; set; } = 0;
        public T Payload { get; set; }

        public PaginationResponse() { }

        public PaginationResponse(int totalRecords, T payload)
        {
            this.TotalRecords = totalRecords;
            this.Payload = payload;
        }
    }
}