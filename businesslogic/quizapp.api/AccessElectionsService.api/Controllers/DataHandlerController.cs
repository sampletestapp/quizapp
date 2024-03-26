﻿using AccessElectionsService.api.Models;
using AccessElectionsService.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccessElectionsService.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataHandlerController : Controller
    {
        private readonly ILogger<DataHandlerController> _logger;
        private readonly ISurveyResponseService _dataLoadService;
        public DataHandlerController(ISurveyResponseService dataLoadService, ILogger<DataHandlerController> logger)
        {
            _dataLoadService = dataLoadService;
            _logger = logger;
        }

        [HttpPost("exportdata")]
        public IActionResult LoadData([FromBody] DataHandler dataHandler)
        {
            _logger.LogDebug("DataHandlerController Data Loading");
            try
            {
                dataHandler.backupFilePath = "D:\\Work\\Tobedeeleted\\Naren\\T_016.bak";
                _dataLoadService.LoadData(dataHandler);
                _logger.LogDebug("DataHandlerController Data Loaded");
                return Ok("Data copy process completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DataHandlerController Data Loading Exception: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("getsurveyresponse")]
        public IActionResult GetSurveyResponse(int pplId, int electionId)
        {
            _logger.LogDebug($"Getting Survey Response for pplId: {pplId} and electionId: {electionId}");
            try
            {
                List<ResponseResultModel> records = _dataLoadService.GetResponseForPPLAndElectionId(pplId, electionId); ;
                _logger.LogDebug($"Obtained Survey Response for pplId: {pplId} and electionId: {electionId}");
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetSurveyResponse Exception: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("updatesurveyresponse")]
        public IActionResult UpdateSurveyResponse([FromBody] UpdateResponseResultModel updatedRecord)
        {
            _logger.LogDebug($"Updating Survey resoponse {updatedRecord.QuestionID}");
            try
            {
                _dataLoadService.UpdatingResponseForQuestion(updatedRecord); ;
                _logger.LogDebug($"Updated Survey resoponse {updatedRecord.QuestionID}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Updating Survey resoponse Exception: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("updateResponseDashboardAvaialbility")]
        public IActionResult UpdateResponseDashboardAvaialbility([FromBody] List<UpdateResponseDashboardAvaialbilityModel> records)
        {

            _logger.LogDebug($"Update Response Dashboard Avaialbility");
            try
            {
                _dataLoadService.UpdateResponseDashboardAvaialbility(records); ;
                _logger.LogDebug($"Update Response Dashboard Avaialbility successfully");
                return Ok("Records updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Update Response Dashboard Avaialbility resoponse Exception");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("updateSurveyStatus")]
        public IActionResult UpdateSurveyStatus([FromBody] SurveyStatusUpdateModel surveyStatusUpdate)
        {
            _logger.LogDebug($"Update Survey Status");
            try
            {
                _dataLoadService.UpdateSurveyStatus(surveyStatusUpdate); ;
                _logger.LogDebug($"Update Survey Status successfully");
                return Ok("Records updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Update Survey Status resoponse Exception");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("getSurveyStatus")]
        public IActionResult GetSurveyStatus(int pplId, int electionId) 
        {
            _logger.LogDebug($"get Survey Status");
            try
            {
                var status = _dataLoadService.GetSurveyStatus(pplId,electionId); ;
                _logger.LogDebug($"get Survey Status successfully");
                return Ok(new { status });
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"get Survey Status resoponse Exception");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
