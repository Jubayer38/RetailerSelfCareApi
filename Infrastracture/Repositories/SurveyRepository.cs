using Domain.Helpers;
using Domain.RequestModel.Survey;
using Infrastracture.DBManagers;
using MySqlConnector;
using System.Data;

///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	11-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************


namespace Infrastracture.Repositories
{
    public class SurveyRepository : IDisposable
    {
        private readonly OracleDbManager _db;
        private MySqlDbManager _mySql;

        public SurveyRepository()
        {
            _mySql = new();
        }

        public SurveyRepository(string connectionString)
        {
            _db = new(connectionString);
        }

        #region==========|  Dispose Method  |==========
        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                _mySql.Dispose();
                _db.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========


        public async Task<int> InsertShortSurveyResponse(SurveyResponse model)
        {
            int retId = Convert.ToInt32(model.retailerCode.Substring(1));

            _mySql.AddParameter(new MySqlParameter("P_SURVEY_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.surveyId });
            _mySql.AddParameter(new MySqlParameter("P_QUESTION_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.questionId });
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.retailerCode });
            _mySql.AddParameter(new MySqlParameter("P_ANSWER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.answer });
            _mySql.AddParameter(new MySqlParameter("P_FILE_PATH", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.filePath });
            _mySql.AddParameter(new MySqlParameter("P_QUESTION_OPTIONS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = model.quesOptions });
            _mySql.AddParameter(new MySqlParameter("P_CREATED_BY", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = retId });
            _mySql.AddParameter(new MySqlParameter("P_IS_SHORT_SURVEY", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = model.isShortSurvey });

            long? result = await _mySql.InsertByStoredProcedureAsync("RSL_INSERT_SURVEY_RESPONSE");
            return (int)result.GetValueOrDefault();
        }

        public async Task<DataTable> GetSurveyValidity(int id, string retailerCode)
        {
            _mySql.AddParameter(new MySqlParameter("P_SURVEY_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = id });
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });

            return await _mySql.CallStoredProcedureSelectAsync("RSL_GET_SURVEY_VALIDITY");
        }

        public async Task<string> GetFileUploadLimit()
        {
            MySqlParameter param = new("P_RETURN", MySqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            _mySql.AddParameter(param);
            await _mySql.CallStoredProcedureSelectAsync("RSL_GET_FILE_UPLOAD_LIMIT");

            return param.Value != DBNull.Value ? param.Value.ToString().ToLower() : string.Empty;
        }


        public async Task<DataTable> GetSurveyQuestionsByID(int id)
        {
            _mySql.AddParameter(new MySqlParameter("P_SURVEY_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = id });
            var result = await _mySql.CallStoredProcedureSelectAsync("RSLGETSP_ALL_SURVEYQNS");
            return result;
        }


        public async Task<DataTable> GetQuestionOptions(int id)
        {
            _mySql.AddParameter(new MySqlParameter("P_QNS_OPTIONS_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = id });
            return await _mySql.CallStoredProcedureSelectAsync("RSLSP_SURVEY_QNSOPTIONS");
        }

        public async Task<DataTable> GetQuestionGridOptions(int id)
        {
            _mySql.AddParameter(new MySqlParameter("P_QNS_OPTIONS_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = id });
            return await _mySql.CallStoredProcedureSelectAsync("RSLSP_SURVEY_QNS_GRIDOPTIONS");
        }

        public async Task DeleteSurveyResponse(int surveyId, string retailerCode)
        {
            _mySql.AddParameter(new MySqlParameter("P_SURVEY_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = surveyId });
            _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = retailerCode });

            await _mySql.CallStoredProcedureSelectAsync("RSLSPDELTE_SURVEY_RESPONSE");
        }

        public async Task<int> InsertSurveyResponse(List<SurveyResponse> model, string retailerCode, int userId)
        {
            //var tuple = _db.BeginTransaction();
            try
            {
                int resultCount = 0;
                foreach (var item in model)
                {
                    _mySql = new MySqlDbManager();
                    item.userId = userId;
                    long ansResp = 0;

                    // Submit main answer model
                    int retId = Convert.ToInt32(retailerCode.Substring(1));
                    // tuple.Item3.Parameters.Clear();
                    _mySql.AddParameter(new MySqlParameter("P_SURVEY_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = item.surveyId });
                    _mySql.AddParameter(new MySqlParameter("P_QUESTION_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = item.questionId });
                    _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });
                    _mySql.AddParameter(new MySqlParameter("P_ANSWER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = item.answer });
                    _mySql.AddParameter(new MySqlParameter("P_FILE_PATH", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = item.filePath, IsNullable = true });
                    _mySql.AddParameter(new MySqlParameter("P_QUESTION_OPTIONS", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = item.quesOptions, IsNullable = true });
                    _mySql.AddParameter(new MySqlParameter("P_IS_SHORT_SURVEY", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = item.isShortSurvey });
                    _mySql.AddParameter(new MySqlParameter("P_CREATED_BY", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = retId });

                    long? result = await _mySql.InsertByStoredProcedureAsync("RSL_INSERT_SURVEY_RESPONSE");
                    ansResp = result.GetValueOrDefault();

                    // Submit answer grid model
                    if (item.qnGridAnswers.Count > 0 && ansResp > 0)
                    {
                        foreach (SurveyAnswerGrid ansGrid in item.qnGridAnswers)
                        {
                            _mySql = new MySqlDbManager();
                            // tuple.Item3.Parameters.Clear();
                            _mySql.AddParameter(new MySqlParameter("P_ANSWER_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = ansResp });
                            _mySql.AddParameter(new MySqlParameter("P_QUESTION_ROW", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = ansGrid.QuestionRow });
                            _mySql.AddParameter(new MySqlParameter("P_QN_ROW_ANSWER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = ansGrid.QnRowAnswer });

                            long? resultAnsGrid = await _mySql.InsertByStoredProcedureAsync("RSL_INSERT_SURVEY_ANS_GRID");

                            if (resultAnsGrid.GetValueOrDefault() <= 0)
                            {
                                throw new Exception("One of the Question Grid answer saving failed.");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.file))
                    {
                        string base64Header = "data:" + item.fileMimeType + ";base64,";
                        string _file = item.file.Replace(base64Header, string.Empty);

                        // tuple.Item3.Parameters.Clear();
                        _mySql.AddParameter(new MySqlParameter("P_SURVEY_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = item.surveyId });
                        _mySql.AddParameter(new MySqlParameter("P_QUESTION_ID", MySqlDbType.Decimal) { Direction = ParameterDirection.Input, Value = item.questionId });
                        _mySql.AddParameter(new MySqlParameter("P_RETAILER_CODE", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = retailerCode });
                        _mySql.AddParameter(new MySqlParameter("P_BASE64_HEADER", MySqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = base64Header });
                        _mySql.AddParameter(new MySqlParameter("P_BASE64_DATA", MySqlDbType.LongBlob) { Direction = ParameterDirection.Input, Value = _file });

                        long? res = await _mySql.InsertByStoredProcedureAsync("RSLSP_INS_SURVEY_QN_FILES");
                    }

                    if (ansResp > 0)
                    {
                        resultCount += 1;
                    }
                }

                if (resultCount != model.Count())
                {
                    throw new Exception("Unable to save survey response. Please try again.");
                }

                // tuple.Item2.Commit();
                return 0;
            }
            catch (Exception ex)
            {
                // tuple.Item2.Rollback();
                throw new Exception(HelperMethod.ExMsgBuild(ex, "InsertSurveyResponseV2"));
            }
            finally
            {
                // if (tuple.Item1.State != ConnectionState.Closed) tuple.Item1.Close();
            }
        }


    }
}
