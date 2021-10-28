using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.NotificationDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Helper;
using kroniiapi.Hubs;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly IHubContext<NotifyHub> _notifyHub;
        private readonly ICacheProvider _cacheProvider;

        private readonly IClassService _classService;
        private readonly ITraineeService _traineeService;

        public NotifyController(IHubContext<NotifyHub> notifyHub, ICacheProvider cacheProvider,
            IClassService classService, ITraineeService traineeService)
        {
            _traineeService = traineeService;
            _classService = classService;
            _notifyHub = notifyHub;
            _cacheProvider = cacheProvider;
        }

        /// <summary>
        /// invoke function ReceiveHistory and send data 
        /// </summary>
        /// <param name="email">email of user want to recieveHistory</param>
        /// <returns>404: email not found / 204: send success</returns>
        [HttpGet("history")]
        public async Task<ActionResult> SendHistory([FromQuery]string email, [FromQuery] PaginationParameter paginationParameter)
        {
            email = email.ToLower();
            List<NotifyMessage> history = new List<NotifyMessage>();
            history = await _cacheProvider.GetFromCache<List<NotifyMessage>>(email);
            if (history == null)
            {
                return NotFound(new ResponseDTO(404, "history not found!"));
            }

            history.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
       
            history = history.Where(t =>
                t.User.Contains(paginationParameter.SearchName.ToLower()) ||
                t.SendTo.Contains(paginationParameter.SearchName.ToLower())).Select(t => t).ToList();

            return Ok(new PaginationResponse<IEnumerable<NotifyMessage>>(history.Count(), PaginationHelper.GetPage(history, paginationParameter)));
        }

        [HttpGet("historyForAdmin")]
        public async Task<ActionResult<IEnumerable<NotifyMessage>>> GetHistoryForAdmin([FromQuery]PaginationParameter paginationParameter)
        {
            List<NotifyMessage> history = new List<NotifyMessage>();
            history = await _cacheProvider.GetAllValueFromCache<NotifyMessage>();
            if (history == null)
            {
                return NotFound(new ResponseDTO(404, "history not found!"));
            }

            history.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));

            history = history.Where(t =>
                t.User.Contains(paginationParameter.SearchName.ToLower()) ||
                t.SendTo.Contains(paginationParameter.SearchName.ToLower())).Select(t => t).ToList();

            return Ok(new PaginationResponse<IEnumerable<NotifyMessage>>(history.Count(), PaginationHelper.GetPage(history, paginationParameter)));
        }

        [HttpPost("setSeen")]
        public async Task<ActionResult> SetSeen(string email)
        {
            if(email == null)
            {
                return BadRequest(new ResponseDTO(404, "email not found"));
            }
            List<NotifyMessage> history = new List<NotifyMessage>();
            history = await _cacheProvider.GetFromCache<List<NotifyMessage>>(email);
            if(history == null)
            {
                return BadRequest(new ResponseDTO(404, "history not found"));
            }
            foreach (var notify in history)
            {
                notify.IsSeen = true;
            }
            await _cacheProvider.SetCache<List<NotifyMessage>>(email, history);

            return Ok(new ResponseDTO(204, "Successfully setSeen"));
        }

        /// <summary>
        /// invoke function ReceiveNotification and send data to all trainee in class
        /// </summary>
        /// <param name="notifyMessage">user: email of admin / content: content / sendTo: class name</param>
        /// <returns>400: error with class name / 204: send sucess</returns>
        [HttpPost("class")]
        public async Task<ActionResult> SendClassNotification([FromBody] NotifyMessage notifyMessage)
        {
            Class classInfor = await _classService.GetClassByClassName(notifyMessage.SendTo);
            notifyMessage.User = notifyMessage.User.ToLower();
            if (classInfor != null)
            {
                IEnumerable<Trainee> traineeList = await _traineeService.GetTraineeByClassId(classInfor.ClassId);
                if (traineeList != null)
                {
                    foreach (var trainee in traineeList)
                    {
                        notifyMessage.SendTo = trainee.Email.ToLower();
                        await _cacheProvider.AddValueToKey<NotifyMessage>(trainee.Email.ToLower(), notifyMessage);
                        await _notifyHub.Clients.Group(trainee.Email.ToLower()).SendAsync("ReceiveNotification", notifyMessage);
                    }
                }
                else
                {
                    return BadRequest(new ResponseDTO(400, "Class is empty"));
                }
            }
            else
            {
                return BadRequest(new ResponseDTO(400, "Can not find class name"));
            }

            return Ok(new ResponseDTO(204, "Successfully invoke"));
        }

        /// <summary>
        /// invoke function ReceiveNotification and send data to trainee
        /// </summary>
        /// <param name="notifyMessage">user: email of admin / content: content / sendTo: trainee's email</param>
        /// <returns>400: cannot find trainee's email / 204: send success</returns>
        [HttpPost("trainee")]
        public async Task<ActionResult> SendTraineeNotification([FromBody] NotifyMessage notifyMessage)
        {
            // if(_traineeService.GetTraineeByEmail(notifyMessage.sendTo) == null )
            // {
            //     return BadRequest(new ResponseDTO(400, "Can not find trainee's email"));
            // }
            notifyMessage.SendTo = notifyMessage.SendTo.ToLower();
            notifyMessage.User = notifyMessage.User.ToLower();
            await _cacheProvider.AddValueToKey<NotifyMessage>(notifyMessage.SendTo, notifyMessage);
            await _notifyHub.Clients.Group(notifyMessage.SendTo.ToLower()).SendAsync("ReceiveNotification", notifyMessage);
            return Ok(new ResponseDTO(204, "Successfully invoke"));
        }
    }
}