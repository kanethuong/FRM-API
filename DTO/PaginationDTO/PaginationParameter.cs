using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.PaginationDTO
{
    public class PaginationParameter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        private string _searchName = "";
        public string SearchName
        {
            get { return _searchName; }
            set
            {
                value.Trim();
                if (value != null) _searchName = value.Replace(" ", ":*|") + ":*";
                else _searchName = "";
            }
        }
    }
}