using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;

namespace kroniiapi.Helper
{
    public class RoleList : IRoleList
    {
        private IEnumerable<Role> roleList;

        public RoleList(DataContext dataContext)
        {
            roleList = dataContext.Roles.ToList();
        }

        public string getRoleName(int id)
        {
            
            foreach (var item in roleList)
            {
                if(item.RoleId == id)
                {
                    return item.RoleName;
                }
            }
            return "";
        }
    }
}