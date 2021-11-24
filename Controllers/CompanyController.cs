using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using kroniiapi.DB.Models;
using kroniiapi.DTO;
using kroniiapi.DTO.CompanyDTO;
using kroniiapi.DTO.PaginationCompanyDTO;
using kroniiapi.DTO.PaginationDTO;
using kroniiapi.DTO.TraineeDTO;
using kroniiapi.Helper;
using kroniiapi.Helper.Upload;
using kroniiapi.Helper.UploadDownloadFile;
using kroniiapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace kroniiapi.Controllers
{
    [ApiController]
    //[Authorize(Policy = "Company")]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {

        private readonly ICompanyService _companyService;
        private readonly ITraineeService _traineeService;
        private readonly IMapper _mapper;
        private readonly IImgHelper _imgHelper;
        private readonly IMegaHelper _megaHelper;
        public CompanyController(IMapper mapper,
                                 ICompanyService companyService,
                                 ITraineeService traineeService,
                                 IImgHelper imgHelper,
                                 IMegaHelper megaHelper)
        {
            _mapper = mapper;
            _companyService = companyService;
            _traineeService = traineeService;
            _imgHelper = imgHelper;
            _megaHelper = megaHelper;
        }

        /// <summary>
        /// View Company profile
        /// </summary>
        /// <param name="companyId">Company id</param>
        /// <returns>Company profile / 404: Profile not found</returns>
        [HttpGet("{companyId:int}/profile")]
        public async Task<ActionResult<CompanyProfileDetail>> ViewCompanyProfile(int companyId)
        {
            Company company = await _companyService.GetCompanyById(companyId);
            if (company == null)
            {
                return NotFound(new ResponseDTO(404, "Company profile cannot be found"));
            }
            return Ok(_mapper.Map<CompanyProfileDetail>(company));
        }

        /// <summary>
        /// View all company request with pagination
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("request")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CompanyRequestResponse>>>> ViewCompanyRequestList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<CompanyRequestResponse> companyRequestResponses) = await _companyService.GetCompanyRequestList(paginationParameter);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Company not found!"));
            }
            return Ok(new PaginationResponse<IEnumerable<CompanyRequestResponse>>(totalRecords, companyRequestResponses));
        }

        /// <summary>
        /// View all company report with pagination (CompanyRequest with isAccepted == true)
        /// </summary>
        /// <param name="paginationParameter"></param>
        /// <returns>200: Total record, list of report / 404: Searched company cannot be found</returns>
        [HttpGet("report")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<CompanyReport>>>> ViewCompanyReportList([FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<CompanyReport> reportList) = await _companyService.GetCompanyReportList(paginationParameter);

            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Company report not found"));
            }

            return Ok(new PaginationResponse<IEnumerable<CompanyReport>>(totalRecord, reportList));
        }

        /// <summary>
        /// View Company Request Detail 
        /// </summary>
        /// <param name="id">company request id</param>
        /// <returns></returns>
        [HttpGet("request/{id:int}")]
        public async Task<ActionResult<RequestDetail>> ViewCompanyRequestDetail(int id)
        {
            var companyRequest = await _companyService.GetCompanyRequestDetail(id);
            if (companyRequest == null)
            {
                return NotFound(new ResponseDTO(404, "Company request not found!"));
            }
            RequestDetail requestDetail = _mapper.Map<RequestDetail>(companyRequest);
            return Ok(requestDetail);
        }

        /// <summary>
        /// View all trainee in company request with pagination
        /// </summary>
        /// <param name="id">company request id</param>
        /// <param name="paginationParameter"></param>
        /// <returns></returns>
        [HttpGet("request/{id:int}/trainee")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<TraineeInRequest>>>> ViewTraineeListInRequest(int id, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecords, IEnumerable<Trainee> trainees) = await _companyService.GetTraineesByCompanyRequestId(id, paginationParameter);
            IEnumerable<TraineeInRequest> traineeDTO = _mapper.Map<IEnumerable<TraineeInRequest>>(trainees);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<TraineeInRequest>>(totalRecords, traineeDTO));
        }

        /// <summary>
        /// Send accept or reject company request
        /// </summary>
        /// <param name="id">company request id</param>
        /// <param name="isAccepted">accept or reject</param>
        /// <returns></returns>
        [HttpPut("request/{id:int}")]
        public async Task<ActionResult> ConfirmCompanyRequest(int id, [FromBody] ConfirmCompanyRequestInput input)
        {
            var rs = await _companyService.ConfirmCompanyRequest(id, input.isAccepted);
            if (rs.Item1 == -1)
            {
                return NotFound(new ResponseDTO(404, "Company request cannot be found"));
            }
            else if (rs.Item1 == -2)
            {
                return BadRequest(new ResponseDTO(400, "Company request had been confirmed before"));
            }
            else if (rs.Item1 == -3)
            {
                return BadRequest(new ResponseDTO(400, $"There are trainees in this list is in onboard: {string.Join(", ", rs.Item2)}"));
            }
            else if (rs.Item1 != 0)
            {
                return Ok(new ResponseDTO(200, "The company request is processed"));
            }
            else return BadRequest(new ResponseDTO(400, "Fail to update"));
        }

        /// <summary>
        /// Search and show trainee list (show all trainees by default)
        /// </summary>
        /// <param name="paginationCompanyParameter">Pagination has 3 search fields</param>
        /// <returns> 200: Total record, list results / 404: Not found </returns>
        [HttpGet("trainee")]
        public async Task<ActionResult<PaginationCompanyResponse<IEnumerable<TraineeSearchResponse>>>> SearchTraineeList([FromQuery] PaginationCompanyParameter paginationCompanyParameter)
        {
            (int totalRecords, IEnumerable<Trainee> trainees) = await _traineeService.GetAllTrainee(paginationCompanyParameter);
            if (totalRecords == 0)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found"));
            }
            IEnumerable<TraineeSearchResponse> traineeDTO = _mapper.Map<IEnumerable<TraineeSearchResponse>>(trainees);
            return Ok(new PaginationCompanyResponse<IEnumerable<TraineeSearchResponse>>(totalRecords, traineeDTO));
        }

        /// <summary>
        /// List all skills (module) of trainee and the finish date of modules
        /// </summary>
        /// <param name="traineeId"></param>
        /// <returns> 200: List of skills / 404: Trainee not found </returns>
        [HttpGet("trainee/{traineeId:int}/skill")]
        public async Task<ActionResult<IEnumerable<TraineeSkillResponse>>> ViewTraineeSkill(int traineeId)
        {
            if (await _traineeService.GetTraineeById(traineeId) == null)
            {
                return NotFound(new ResponseDTO(404, "Trainee not found"));
            }
            var tSRList = await _traineeService.GetTraineeSkillByTraineeId(traineeId);
            return Ok(tSRList);
        }

        /// <summary>
        /// Send request a list of trainees
        /// </summary>
        /// <param name="requestTraineeInput"></param>
        /// <returns> 200: Send success / 409: Fail to send request</returns>
        [HttpPost("request")]
        public async Task<ActionResult> SendTraineeRequest(RequestTraineeInput requestTraineeInput)
        {
            var checkInputList = requestTraineeInput.CompanyRequestDetails;
            if (checkInputList == null || checkInputList.Count() == 0)
            {
                return Conflict(new ResponseDTO(409,"Trainee list cannot be empty!"));
            }
            List<int> acceptedTraineeId = await _companyService.GetAcceptedTraineeIdList();
            var error = "Trainee(s) has been approved to another company: ";
            var count = 0;
            foreach (var item in requestTraineeInput.CompanyRequestDetails)
            {
                var checkTrainee = await _traineeService.GetTraineeById(item.TraineeId);
                if (checkTrainee == null)
                {
                    return NotFound(new ResponseDTO(404, "Trainee not found"));
                }
                foreach (var traineeId in acceptedTraineeId)
                {
                    if (item.TraineeId == traineeId)
                    {
                        var trainee = await _traineeService.GetTraineeById(traineeId);
                        error += trainee.Fullname ;
                        error += ", ";
                        count++;
                    }
                }
            }
            error = error.Remove(error.Length-2);
            if (count != 0)
            {
                return Conflict(new ResponseDTO(409, error));
            }

            CompanyRequest companyRequest = _mapper.Map<CompanyRequest>(requestTraineeInput);

            var rs = await _companyService.InsertNewCompanyRequestIncludeTrainee(companyRequest);
            if (rs == 0)
            {
                return BadRequest(new ResponseDTO(409, "Failed to send request"));
            }
            return Ok(new ResponseDTO(200, "Send Success"));
        }

        /// <summary>
        /// View list of trainee request by company id
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="paginationParameter"></param>
        /// <returns> 200: Total record,list of request / 404: Company not found</returns>
        [HttpGet("{companyId:int}/request")]
        public async Task<ActionResult<PaginationResponse<IEnumerable<RequestTraineeResponse>>>> ViewTraineeRequestList(int companyId, [FromQuery] PaginationParameter paginationParameter)
        {
            (int totalRecord, IEnumerable<CompanyRequest> companyRequests) = await _companyService.GetTraineeListInRequestByCompanyId(companyId, paginationParameter);
            IEnumerable<RequestTraineeResponse> requestTraineeResponses = _mapper.Map<IEnumerable<RequestTraineeResponse>>(companyRequests);
            if (totalRecord == 0)
            {
                return NotFound(new ResponseDTO(404, "Company not found"));
            }
            return Ok(new PaginationResponse<IEnumerable<RequestTraineeResponse>>(totalRecord, requestTraineeResponses));
        }

        /// <summary>
        /// View detail of a request of company
        /// </summary>
        /// <param name="companyId">Company id</param>
        /// <param name="requestId">Company Request id</param>
        /// <returns> 200: Request detail / 404: ID not found </returns>
        [HttpGet("{companyId:int}/{requestId:int}")]
        public async Task<ActionResult<RequestTraineeDetailResponse>> ViewRequestDetail(int companyId, int requestId)
        {
            if (await _companyService.GetCompanyById(companyId) == null)
            {
                return NotFound(new ResponseDTO(404, "Company not found!"));
            }
            if (await _companyService.GetCompanyRequestById(requestId) == null)
            {
                return NotFound(new ResponseDTO(404, "Request not found!"));
            }
            var companyRequest = await _companyService.GetRequestDetailByCompanyIdAndRequestId(companyId, requestId);
            if (companyRequest == null)
            {
                return NotFound(new ResponseDTO(404, "Company request not found!"));
            }
            RequestTraineeDetailResponse requestDetail = _mapper.Map<RequestTraineeDetailResponse>(companyRequest);
            return Ok(requestDetail);
        }

        /// <summary>
        /// Upload report of a request
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="file"></param>
        /// <returns> 200: Upload success / 404: ID not found / 409: Fail to upload </returns>
        [HttpPost("request/{requestId:int}/report")]
        public async Task<ActionResult> UploadReport(int requestId, [FromForm] IFormFile file)
        {
            CompanyRequest companyRequest = await _companyService.GetCompanyRequestById(requestId);
            if (companyRequest == null)
            {
                return NotFound(new ResponseDTO(404, "Request is not exist"));
            }

            if (_companyService.IsCompanyRequestAccepted(requestId) != true)
            {
                return Conflict(new ResponseDTO(409, "Request has not been accepted or has been rejected"));
            }

            (bool isExcel, string errorMsg) = FileHelper.CheckExcelExtension(file);
            if (isExcel == false)
            {
                return Conflict(new ResponseDTO(409, errorMsg));
            }

            Stream stream = file.OpenReadStream();
            string reportUrl = await _megaHelper.Upload(stream, file.FileName, "Company Report");
            int rs = await _companyService.UpdateCompanyReportUrl(requestId, reportUrl);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to upload report"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Upload report success"));
            }
        }

        /// <summary>
        /// Edit company profile
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="companyProfileDetailInput"></param>
        /// <returns> 200: Edit success / 404: Company not found / 409: Edit fail</returns>
        [HttpPut("{companyId:int}/profile")]
        public async Task<ActionResult> EditProfile(int companyId, [FromBody] CompanyProfileDetailInput companyProfileDetailInput)
        {
            Company company = _mapper.Map<Company>(companyProfileDetailInput);
            Company existedCompany = await _companyService.GetCompanyById(companyId);
            if (existedCompany == null)
            {
                return NotFound(new ResponseDTO(404, "Company profile cannot be found"));
            }
            if (
                existedCompany.Fullname.ToLower().Equals(company.Fullname.ToLower()) &&
                existedCompany.Phone.ToLower().Equals(company.Phone.ToLower()) &&
                existedCompany.Address.ToLower().Equals(company.Address.ToLower()) &&
                existedCompany.Facebook.ToLower().Equals(company.Facebook.ToLower())
            )
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
            int rs = await _companyService.UpdateCompany(companyId, company);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to update company profile"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update profile success"));
            }
        }

        /// <summary>
        /// Update company avatar
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="image"></param>
        /// <returns> 200: Update success / 404: Company not found / 409: Update fail </returns>
        [HttpPut("{companyId:int}/avatar")]
        public async Task<ActionResult> UpdateAvatar(int companyId, IFormFile image)
        {
            (bool isImage, string errorMsg) = FileHelper.CheckImageExtension(image);
            if (isImage == false)
            {
                return Conflict(new ResponseDTO(409, errorMsg));
            }
            var check = await _companyService.GetCompanyById(companyId);
            {
                if (check == null)
                {
                    return NotFound(new ResponseDTO(404, "Company profile cannot be found"));
                }
            }
            string fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.Trim('"');
            Stream stream = image.OpenReadStream();
            long fileLength = image.Length;
            string fileType = image.ContentType;

            string avatarUrl = await _imgHelper.Upload(stream, fileName, fileLength, fileType);
            int rs = await _companyService.UpdateAvatar(companyId, avatarUrl);
            if (rs == 0)
            {
                return Conflict(new ResponseDTO(409, "Fail to update Company avatar"));
            }
            else
            {
                return Ok(new ResponseDTO(200, "Update avatar success"));
            }
        }
    }
}