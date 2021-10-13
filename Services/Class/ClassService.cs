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
            var listClass = await _dataContext.Classes
                                    .Where(c => c.IsDeactivated == false)
                                    .Select(c => new Class
                                    {
                                        ClassId = c.ClassId,
                                        ClassName = c.ClassName,
                                        Description = c.Description,
                                        CreatedAt = c.CreatedAt,
                                        IsDeactivated = c.IsDeactivated,
                                        DeactivatedAt = c.DeactivatedAt,
                                        Trainees = c.Trainees,
                                        AdminId = c.AdminId,
                                        Admin = c.Admin,
                                        TrainerId = c.TrainerId,
                                        Trainer = c.Trainer,
                                        RoomId = c.RoomId,
                                        Room = c.Room,
                                        Modules = c.Modules,
                                    }).ToListAsync();

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
            var classGet = await (
                from c in _dataContext.Classes
                where c.ClassName == className
                select new Class
                {
                    ClassId = c.ClassId,
                    ClassName = className,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    IsDeactivated = c.IsDeactivated,
                    DeactivatedAt = c.DeactivatedAt,
                    Trainees = c.Trainees,
                    Room = c.Room,
                    Modules = c.Modules,
                    Calendars = c.Calendars,
                }).FirstOrDefaultAsync();
            return classGet;
        }
        /// <summary>
        /// Get Request Deleted Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>Tuple List of  Class Delete Request</returns>
        public async Task<Tuple<int, IEnumerable<DeleteClassRequest>>> GetRequestDeleteClassList(PaginationParameter paginationParameter)
        {
            var listRequest = await _dataContext.DeleteClassRequests
                                    .Select(c => new DeleteClassRequest
                                    {
                                        DeleteClassRequestId = c.DeleteClassRequestId,
                                        Reason = c.Reason,
                                        CreatedAt = c.CreatedAt,
                                        IsAccepted = c.IsAccepted,
                                        AcceptedAt = c.AcceptedAt,
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
        /// <returns>True if Success to Change & False if false to change</returns>
        public async Task<int> UpdateDeletedClass(ConfirmDeleteClassInput confirmDeleteClassInput)
        {
            if (confirmDeleteClassInput.IsDeactivate == true)
            {
                // get Class in Input
                var existedClass = await _dataContext.Classes.Where(i => i.ClassId == confirmDeleteClassInput.ClassId).FirstOrDefaultAsync();
                if (existedClass == null)
                {
                    return -1;
                }
                existedClass.IsDeactivated = true;
                // get Request in Input
                var existedRequest = await _dataContext.DeleteClassRequests.Where(d => d.DeleteClassRequestId == confirmDeleteClassInput.DeleteClassRequestId).FirstOrDefaultAsync();
                if (existedRequest == null)
                {
                    return -1;
                }
                existedRequest.IsAccepted = true;
                // Save Change 
                var rs = await _dataContext.SaveChangesAsync();
                if (rs == 1)
                {
                    return 1;
                }
            } else
            {
                return 0;
            }
        }
        /// <summary>
        ///  Get Deleted Class List
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns> Tuple List of Deleted Class </returns>
        public async Task<Tuple<int, IEnumerable<Class>>> GetDeletedClassList(PaginationParameter paginationParameter)
        {
            var listClass = await _dataContext.Classes
                                    .Where(c => c.IsDeactivated == true)
                                    .Select(c => new Class
                                    {
                                        ClassId = c.ClassId,
                                        ClassName = c.ClassName,
                                        Description = c.Description,
                                        CreatedAt = c.CreatedAt,
                                        IsDeactivated = c.IsDeactivated,
                                        DeactivatedAt = c.DeactivatedAt,
                                        Trainees = c.Trainees,
                                        AdminId = c.AdminId,
                                        Admin = c.Admin,
                                        TrainerId = c.TrainerId,
                                        Trainer = c.Trainer,
                                        RoomId = c.RoomId,
                                        Room = c.Room,
                                        Modules = c.Modules,
                                    }).ToListAsync();

            int totalRecords = listClass.Count();

            var rs = listClass.OrderBy(c => c.ClassId)
                     .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                     .Take(paginationParameter.PageSize);

            return Tuple.Create(totalRecords, rs);
        }
    }
}