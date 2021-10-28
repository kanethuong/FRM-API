using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.ClassDetailDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainerController : ControllerBase
    {
        private readonly ITrainerService _trainerService;
        private readonly IMapper _mapper;
        public TrainerController(IMapper mapper,ITrainerService trainerService)
        {
            _mapper = mapper;
            _trainerService = trainerService;
        }
        [HttpGet("free_trainer")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TrainerResponse>>>> GetAllTrainerInForm([FromQuery]PaginationParameter paginationParameter)
        {
            
            (int totalRecord, IEnumerable<Trainer> listTrainer) = await _trainerService.GetAllTrainer(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Search trainer email not found"));
            }
            var trainers = _mapper.Map<IEnumerable<TrainerResponse>>(listTrainer);

            return Ok(new PaginationResponse<IEnumerable<TrainerResponse>>(totalRecord, trainers));
        }
    }
}