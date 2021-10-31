using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;
using kroniiapi.DB.Models;
using kroniiapi.DTO.PaginationDTO;
using Microsoft.EntityFrameworkCore;

namespace kroniiapi.Services
{
    public class ExamService : IExamService
    {
        private DataContext _dataContext;
        public ExamService(DataContext dataContext )
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Get exam method by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Exam data</returns>
        public async Task<Exam> GetExamById(int id)
        {
            return await _dataContext.Exams.Where(e => e.ExamId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Insert new exam method
        /// </summary>
        /// <param name="exam"></param>
        /// <returns>-1:existed / 0:fail / 1:success</returns>
        public async Task<int> InsertNewExam(Exam exam){
            if(_dataContext.Exams.Any(e =>
                e.ExamId == exam.ExamId
            ))
            {
                return -1;
            }
            int rowInserted = 0;

            _dataContext.Exams.Add(exam);

            rowInserted = await _dataContext.SaveChangesAsync();

            return rowInserted;
        }
        
        /// <summary>
        /// Update exam method
        /// </summary>
        /// <param name="id"></param>
        /// <param name="exam"></param>
        /// <returns>-1:none existed / 0:fail / 1:success</returns>
        public async Task<int> UpdateExam(int id, Exam exam){
            var existedExam = await _dataContext.Exams.Where(t => t.ExamId == id).FirstOrDefaultAsync();
            if(existedExam == null)
            {
                return -1;
            }
            existedExam.ExamName = exam.ExamName;
            existedExam.Description = exam.Description;
            existedExam.ExamDay = exam.ExamDay;
            existedExam.DurationInMinute = exam.DurationInMinute;

            var rowUpdated = await _dataContext.SaveChangesAsync();

            return rowUpdated;
            
        }

        /// <summary>
        /// Pagination Get Exam List method
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>Exam list</returns>
        public async Task<Tuple<int, IEnumerable<Exam>>> GetExamList(PaginationParameter paginationParameter){
             var listExam = await _dataContext.Exams.Where(e => e.ExamName.ToUpper().Contains(paginationParameter.SearchName.ToUpper())).ToListAsync();

            int totalRecords = listExam.Count();

            var rs = listExam.OrderBy(e => e.ExamId)
                     .Skip((paginationParameter.PageNumber - 1) * paginationParameter.PageSize)
                     .Take(paginationParameter.PageSize);

            return Tuple.Create(totalRecords, rs);
        }

        public async Task<IEnumerable<Exam>> GetExamListByModuleId(List<Calendar> calendars, DateTime startDate, DateTime endDate){
            List<int> mds = new List<int>();
            foreach (var item in calendars)
            {
                mds.Add(item.ModuleId);
            }
            List<int> mdsNodup = mds.Distinct().ToList();
            List<Exam> exams = new List<Exam>();
            foreach (var i in mdsNodup)
            {
                var e = await _dataContext.Exams.Where(e => e.ModuleId == i && e.ExamDay >= startDate && e.ExamDay <= endDate).Select(
                    e => new Exam{
                        ExamId = e.ExamId,
                        ExamName = e.ExamName,
                        Description = e.Description,
                        ExamDay = e.ExamDay,
                        DurationInMinute = e.DurationInMinute,
                        ModuleId = e.ModuleId,
                        Module = new Module{
                            ModuleName = e.Module.ModuleName
                        }
                
                }
                ).ToListAsync();
                exams.AddRange(e);
            }
            return exams;
        }
            public async Task<IEnumerable<Exam>> GetExamListByTraineeId(int traineeId, DateTime startDate, DateTime endDate){
            var traineeExams = await _dataContext.TraineeExams.Where(e => e.TraineeId == traineeId).Select(e => e.ExamId).ToListAsync();
            List<Exam> exams = new List<Exam>();
            foreach (var item in traineeExams)
            {
                var e = await _dataContext.Exams.Where(e => e.ExamId == item && e.ExamDay >= startDate && e.ExamDay <= endDate && e.IsCancelled == false).Select(
                    e => new Exam{
                        ExamId = e.ExamId,
                        ExamName = e.ExamName,
                        Description = e.Description,
                        ExamDay = e.ExamDay,
                        DurationInMinute = e.DurationInMinute,
                        ModuleId = e.ModuleId,
                        Module = new Module{
                            ModuleName = e.Module.ModuleName
                        },
                        AdminId = e.AdminId,
                        Admin = new Admin {
                            AdminId = e.Admin.AdminId,
                            Fullname = e.Admin.Fullname,
                            AvatarURL = e.Admin.AvatarURL,
                            Email = e.Admin.Email
                        },
                        

                }
                ).ToListAsync();
                exams.AddRange(e);
            }
            var examsOrdered = exams.OrderBy(e => e.ExamDay);
            return examsOrdered;
            }
    }
}