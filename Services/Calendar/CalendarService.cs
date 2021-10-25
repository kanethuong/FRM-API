using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB;

namespace kroniiapi.Services.Calendar
{
    public class CalendarService:ICalendarService
    {
        private readonly DataContext _dataContext;
        public CalendarService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
    }
}