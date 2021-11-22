using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kroniiapi.DTO.PaginationCompanyDTO
{
    public class PaginationCompanyParameter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        private string _searchName = "";
        private string _searchSkill = "";
        private string _searchLocation = "";
        public string SearchName
        {
            get { return _searchName; }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                    _searchName = value.Replace(" ", ":*|") + ":*";
                }
                else _searchName = "";
            }
        }

        public string SearchSkill
        {
            get { return _searchSkill; }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                    _searchSkill = value.Replace(" ", ":*|") + ":*";
                }
                else _searchSkill = "";
            }
        }

        public string SearchLocation
        {
            get { return _searchLocation; }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                    _searchLocation = value.Replace(" ", ":*|") + ":*";
                }
                else _searchLocation = "";
            }
        }
    }
}