using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class ClassService : IClassService
    {
        private DataContext _dataContext;

        public ClassService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Get Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple List of Class List </returns>
        public async Task<Tuple<int, IEnumerable<Class>>> GetClassList(PaginationParameter paginationParameter)
        {
            var listClass = await _dataContext.Classes.Where(c => c.IsDeactivated == false && c.ClassName.ToUpper().Contains(paginationParameter.SearchName.ToUpper())).ToListAsync();

            int totalRecords = listClass.Count();

            var rs = listClass.OrderBy(c => c.ClassId)
                     .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                     .Take(paginationParameter.PageSize);

            return Tuple.Create(totalRecords, rs);
        }
        /// <summary>
        /// Get Class By ClassName
        /// </summary>
        /// <param name="className"></param>
        /// <returns> Class </returns>
        public async Task<Class> GetClassByClassName(string className)
        {
            return await _dataContext.Classes.Where(c => c.ClassName == className).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get Request Deleted Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>Tuple List of  Class Delete Request</returns>
        public async Task<Tuple<int, IEnumerable<DeleteClassRequest>>> GetRequestDeleteClassList(PaginationParameter paginationParameter)
        {
            var listRequest = await _dataContext.DeleteClassRequests
                                    .Where(c => c.IsAccepted == null)
                                    .Select(c => new DeleteClassRequest
                                    {
                                        DeleteClassRequestId = c.DeleteClassRequestId,
                                        Reason = c.Reason,
                                        CreatedAt = c.CreatedAt,
                                        IsAccepted = c.IsAccepted,
                                        AcceptedAt = c.AcceptedAt,
                                        ClassId = c.ClassId,
                                        Class = new Class
                                        {
                                            ClassId = c.ClassId,
                                            ClassName = c.Class.ClassName,
                                            Description = c.Class.Description,
                                            CreatedAt = c.Class.CreatedAt,
                                        },
                                        Admin = new Admin
                                        {
                                            AdminId = c.AdminId,
                                            Username = c.Admin.Username,
                                            Fullname = c.Admin.Fullname,
                                            AvatarURL = c.Admin.AvatarURL,
                                            Email = c.Admin.Email,
                                        }
                                    }
                                    ).ToListAsync();

            int totalRecords = listRequest.Count();

            var rs = listRequest.OrderBy(c => c.ClassId)
                     .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                     .Take(paginationParameter.PageSize);

            return Tuple.Create(totalRecords, rs);
        }
        /// <summary>
        /// Update Deleted Class
        /// </summary>
        /// <param name="confirmDeleteClassInput"></param>
        /// <returns>1 if Success to Change & 0 if false to change & -1 if invalid & 2 if is rejected</returns>
        public async Task<int> UpdateDeletedClass(ConfirmDeleteClassInput confirmDeleteClassInput)
        {
            if (confirmDeleteClassInput.IsDeactivate == true)
            {
                var existedClass = await _dataContext.Classes.Where(i => i.ClassId == confirmDeleteClassInput.ClassId).FirstOrDefaultAsync();
                if (existedClass == null)
                {
                    return -1;
                }
                var existedRequest = await _dataContext.DeleteClassRequests.Where(d => d.DeleteClassRequestId == confirmDeleteClassInput.DeleteClassRequestId).FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    return -1;
                }
                if (existedClass.IsDeactivated == true || existedRequest.IsAccepted == true)
                {
                    return 0;
                }
                existedClass.IsDeactivated = true;
                existedClass.DeactivatedAt = DateTime.Now;
                existedRequest.IsAccepted = true;
                existedRequest.AcceptedAt = DateTime.Now;
                // Save Change 
                var rs = await _dataContext.SaveChangesAsync();
                if (rs == 1)
                {
                    return 1;
                }
            } else if (confirmDeleteClassInput.IsDeactivate == false)
            {
                var existedRequest = await _dataContext.DeleteClassRequests.Where(d => d.DeleteClassRequestId == confirmDeleteClassInput.DeleteClassRequestId).FirstOrDefaultAsync();
                existedRequest.IsAccepted = false;
                await _dataContext.SaveChangesAsync();
                return 2;
            }
            else if (confirmDeleteClassInput.IsDeactivate == false)
            {
                var existedRequest = await _dataContext.DeleteClassRequests.Where(d => d.DeleteClassRequestId == confirmDeleteClassInput.DeleteClassRequestId).FirstOrDefaultAsync();
                existedRequest.IsAccepted = false;
                await _dataContext.SaveChangesAsync();
                return 2;
            }
            return -1;
        }
        /// <summary>
        ///  Get Deleted Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple List of Deleted Class </returns>
        public async Task<Tuple<int, IEnumerable<Class>>> GetDeletedClassList(PaginationParameter paginationParameter)
        {
            var listClass = await _dataContext.Classes.Where(c => c.IsDeactivated == true).ToListAsync();

            int totalRecords = listClass.Count();

            var rs = listClass.OrderBy(c => c.ClassId)
                     .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                     .Take(paginationParameter.PageSize);

            return Tuple.Create(totalRecords, rs);
        }
        /// <summary>
        /// Get detail of a class 
        /// </summary>
        /// <param name="id">id of the class</param>
        /// <returns></returns>
        /// Note for DatLT: dùng hàm GetTraineesByClassId ở dưới để get pagination cho trainee 
        /// rồi gán vào TraineeList ở ClassDetailResponse trong controller
        public async Task<Class> GetClassDetail(int id)
        {
            return null;
        }

        /// <summary>
        /// Get Trainee List in a class with pagination
        /// </summary>
        /// <param name="id">id of the class</param>
        /// <param name="paginationParameter">pagination param to get approriate trainee in a page</param>
        /// <returns>tuple list of trainee</returns>
        public async Task<Tuple<int, ICollection<Trainee>>> GetTraineesByClassId(int id, PaginationParameter paginationParameter)
        {
            return null;
        }

        /// <summary>
        /// Insert New Request Delete Class to db
        /// </summary>
        /// <param name="requestDeleteClassInput"></param>
        /// <returns>1: done / tu suy nghi tiep nhe KhangTD </returns>
        public async Task<int> InsertNewRequestDeleteClass(DeleteClassRequest deleteClassRequest)
        {
            return 0;
        }
    }
}