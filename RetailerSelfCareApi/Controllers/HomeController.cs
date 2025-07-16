///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Dashbord of API, Also survey views and submit api collection of Retailer App
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.    
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Application.Services;
using Domain.Helpers;
using Domain.RequestModel.Survey;
using Domain.Resources;
using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace RetailerSelfCareApi.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            ViewBag.Title = "Retailer Self Care App API";

            return View();
        }


        [Route(nameof(SurveyView))]
        public async Task<IActionResult> SurveyView(int id, string rc, int isDark = 0)
        {
            SurveyPropertiesModel surveyProp = new();
            string traceMsg = string.Empty;

            SurveyService surveyService;
            DataTable dt = new();

            try
            {
                surveyService = new();
                dt = await surveyService.GetSurveyValidity(id, rc);
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetSurveyValidity"));
            }

            if (dt.Rows.Count > 0)
            {
                int fileSizeLimit = 0;
                #region survey file upload limit getting from db. Currently not used
                //string fileLimitString = string.Empty;

                //try
                //{
                //    surveyService = new();
                //    fileLimitString = await surveyService.GetFileUploadLimit();
                //}
                //catch (Exception ex)
                //{
                //    traceMsg = HelperMethod.BuildTraceMessage(traceMsg, "GetFileUploadLimit", ex);
                //}

                //if (!string.IsNullOrEmpty(fileLimitString))
                //{
                //    fileSizeLimit = !string.IsNullOrEmpty(fileLimitString) ? Convert.ToInt32(Regex.Match(fileLimitString, @"\d+").Value) : 0;
                //    fileSizeLimit = fileSizeLimit != 0 ? fileSizeLimit * 1000000 : fileSizeLimit;
                //}
                #endregion

                surveyProp.Id = Convert.ToInt32(dt.Rows[0]["SURVEY_ID"]);
                surveyProp.Title = dt.Rows[0]["TITLE"] as string;
                surveyProp.Description = dt.Rows[0]["DESCRIPTION"] as string;
                surveyProp.EndDate = Convert.ToDateTime(dt.Rows[0]["END_DATE"]);
                surveyProp.IsValid = Convert.ToInt32(dt.Rows[0]["IS_VALID"]);
                surveyProp.RetailerCode = rc;
                surveyProp.FileUploadLimit = fileSizeLimit;
            }
            else
            {
                surveyProp.Id = id;
                surveyProp.IsValid = 5;
                surveyProp.RetailerCode = rc;
            }

            if (isDark == 1)
            {
                return View("SurveyViewDark", surveyProp);
            }

            if (!string.IsNullOrWhiteSpace(traceMsg))
            {
                LoggerService _logger = new();
                _logger.WriteTraceMessageInText(rc, "SurveyView", traceMsg);
            }

            return View(surveyProp);
        }


        [HttpGet]
        [Route(nameof(GetSurveyQuestionsByID))]
        public async Task<IActionResult> GetSurveyQuestionsByID(int id)
        {
            SurveyService surveyService;
            List<Question> questionList = new();

            try
            {
                using(surveyService = new())
                {
                    questionList = await surveyService.GetAllQuestionsByID(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetAllQuestionsByID"));
            }

            for (int i = 0; i < questionList.Count; i++)
            {
                Question question = questionList[i];

                surveyService = new();
                switch (question.input_type)
                {
                    case "checkbox-single-select":
                    case "checkbox-multiple-select":
                    case "radio":
                    case "dropdown":
                        await surveyService.GetQuestionOptions(question.id, question);
                        questionList[i] = question;
                        break;
                    case "multiple-choice-grid":
                    case "checkbox-grid":
                        await surveyService.GetQuestionGridOptions(question.id, question);
                        questionList[i] = question;
                        break;
                    default:
                        continue;
                }
            }

            return Json(new ResponseMessage()
            {
                isError = false,
                message = SharedResource.GetLocal("Success", Message.Success),
                data = questionList
            });
        }


        [HttpPost]
        [Route(nameof(SubmitSurveyResponse))]
        public async Task<IActionResult> SubmitSurveyResponse(string retailerCode, string surveyAnswers)
        {
            List<SurveyResponse> respList = JsonConvert.DeserializeObject<List<SurveyResponse>>(surveyAnswers);

            SurveyService surveyService;

            int surveyId = respList[0].surveyId;

            try
            {
                using (surveyService = new())
                {
                    await surveyService.DeleteSurveyResponse(surveyId, retailerCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "DeleteSurveyResponse"));
            }

            using (surveyService = new())
            {
                _ = await surveyService.InsertSurveyResponse(respList, retailerCode, UserSession.userId);
            }

            return Json(new ResponseMessage() { isError = false, message = SharedResource.GetLocal("Success", Message.Success) });
        }
    }
}