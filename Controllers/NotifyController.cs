using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.NotificationDTO;
using kroniiapi.DTO.PaginationDTO;
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
        public async Task<ActionResult> SendHistory(string email)
        {
            email = email.ToLower();
            List<NotifyMessage> history = new List<NotifyMessage>();
            history = await _cacheProvider.GetFromCache<List<NotifyMessage>>(email);
            history.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));
            if(history == null)
            {
                return NotFound(new ResponseDTO(404, "history not found!"));
            }
            else
            {
                await _notifyHub.Clients.Group(email).SendAsync("ReceiveHistory", history);
                return Ok( new ResponseDTO(204, "Successfully invoke"));
            }
        }

        /// <summary>
        /// invoke function ReceiveNotification and send data to all trainee in class
        /// </summary>
        /// <param name="notifyMessage">user: email of admin / content: content / sendTo: class name</param>
        /// <returns>400: error with class name / 204: send sucess</returns>
        [HttpPost("class")]
        public async Task<ActionResult> SendClassNotification([FromBody]NotifyMessage notifyMessage)
        {
            Class classInfor = await _classService.GetClassByClassName(notifyMessage.sendTo);
            if (classInfor != null)
            {
                IEnumerable<Trainee> traineeList = await _traineeService.GetTraineeByClassId(classInfor.ClassId);
                if (traineeList != null)
                {
                    foreach (var trainee in traineeList)
                    {
                        notifyMessage.sendTo = trainee.Email;
                        await _cacheProvider.AddValueToKey<NotifyMessage>(trainee.Email, notifyMessage);
                        await _notifyHub.Clients.Group(trainee.Email).SendAsync("ReceiveNotification", notifyMessage);
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
        public async Task<ActionResult> SendTraineeNotification([FromBody]NotifyMessage notifyMessage)
        {
            // if(_traineeService.GetTraineeByEmail(notifyMessage.sendTo) == null )
            // {
            //     return BadRequest(new ResponseDTO(400, "Can not find trainee's email"));
            // }
            await _cacheProvider.AddValueToKey<NotifyMessage>(notifyMessage.sendTo, notifyMessage);
            await _notifyHub.Clients.Group(notifyMessage.sendTo).SendAsync("ReceiveNotification", notifyMessage);
            return Ok(new ResponseDTO(204, "Successfully invoke"));
        }
    }
}