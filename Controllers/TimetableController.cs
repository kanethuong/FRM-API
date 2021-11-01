using System.Threading.Tasks;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDTO;
using kroniiapi.DTO.FeedbackDTO;
using kroniiapi.DTO.MarkDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimetableController : ControllerBase
    {
        private readonly ITimetableService _timetableService;
        private readonly IClassService _classService;
        public TimetableController(ITimetableService timetableService, IClassService classService)
        {
            _timetableService = timetableService;
            _classService = classService;
        }
        [HttpPost("create")]
        public async Task<ActionResult> CreateTimetableForClass( int classId)
        {
            var moduleList = await _timetableService.GetModuleListlByClassId(classId);
            var classGet = await _classService.GetClassByClassID(classId);
            if (_timetableService.CheckAvailableModule(moduleList,classGet.StartDay,classGet.EndDay))
            {
                return BadRequest(new ResponseDTO(404, "Hoc it thoi"));
            }
            int slotsNeed = _timetableService.GetTotalSlotsNeed(moduleList);
            if ( _timetableService.CheckAvailabeSlotsForRoom(slotsNeed,classGet.RoomId,classGet.StartDay,classGet.EndDay))
            {
                var otherRooms = await _timetableService.GetOtherRoomsForClass(slotsNeed, classGet.RoomId, classGet.StartDay, classGet.EndDay);
                string message = "Room is already full of slot, Recommend:";
                foreach (var item in otherRooms)
                {
                    message += " "  + item;
                }
                return Conflict(new ResponseDTO(409, message));
            }
            if (_timetableService.CheckAvailabeSlotsForTrainer(slotsNeed,classGet.TrainerId,classGet.StartDay,classGet.EndDay))
            {
                return Conflict(new ResponseDTO(409, "This Trainer is bussy in that among of time"));
            }
            foreach (var item in moduleList)
            {
                var slotForWeek = _timetableService.CalculateSlotForWeek(item.NoOfSlot, classGet.StartDay, classGet.EndDay);
                var slotss = item.NoOfSlot;
                var status = await _timetableService.InsertModuleToClass(item.ModuleId, classId, slotForWeek);
                if (status == -1)
                {
                    return BadRequest(new ResponseDTO(404, "Can not Insert Modules To Class"));
                }
            }
            return Ok(new ResponseDTO(200, "Successfully"));
        }

    }
}