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

using Domain.Helpers;
using Domain.RequestModel.Survey;
using Domain.ViewModel;
using Infrastracture.Repositories;
using System.Data;


namespace Application.Services
{
    public class SurveyService : IDisposable
    {
        private readonly SurveyRepository _repo;

        public SurveyService()
        {
            _repo = new();
        }

        public SurveyService(string connectionString)
        {
            _repo = new(connectionString);
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
                _repo.Dispose();
            }

            isDisposed = true;
        }
        #endregion==========|  Dispose Method  |==========

        public async Task<int> InsertShortSurveyResponse(SurveyResponse model)
        {
            return await _repo.InsertShortSurveyResponse(model);
        }


        public async Task<DataTable> GetSurveyValidity(int id, string retailerCode)
        {
            return await _repo.GetSurveyValidity(id, retailerCode);
        }

        public async Task<string> GetFileUploadLimit()
        {
            return await _repo.GetFileUploadLimit();
        }


        public async Task<List<Question>> GetAllQuestionsByID(int id)
        {
            DataTable dt = await _repo.GetSurveyQuestionsByID(id);

            return ObjectMappingList(dt);
        }

        public List<Question> ObjectMappingList(DataTable dt)
        {
            List<Question> questions = new();
            foreach (DataRow row in dt.Rows)
            {
                QuestionGridColumn gridColumn = new();
                Question question = new();

                question.id = Convert.ToInt32(row["question_id"] == DBNull.Value ? 0 : row["question_id"]);
                question.ques_descrip = (row["description"] == DBNull.Value ? null : row["description"].ToString());
                question.input_type = (row["input_type"] == DBNull.Value ? null : row["input_type"].ToString());
                question.isRequired = Convert.ToBoolean(row["IS_REQUIRED"]);
                question.quesOrder = Convert.ToInt32(row["QUESTION_ORDER"]);

                switch (question.input_type)
                {
                    case "checkbox-single-select":
                    case "checkbox-multiple-select":
                    case "radio":
                    case "dropdown":
                        var ques_options = row["ques_options"] == DBNull.Value ? null : row["ques_options"].ToString();
                        if (!string.IsNullOrWhiteSpace(ques_options))
                            assignQuesOptions(ques_options.Remove(ques_options.Length - 1, 1), ref question);
                        break;
                    case "linear-scale":
                        question.linear_start_value = Convert.ToInt32(row["linear_start_value"] == DBNull.Value ? 0 : row["linear_start_value"]);
                        question.linear_end_value = Convert.ToInt32(row["linear_end_value"] == DBNull.Value ? 0 : row["linear_end_value"]);
                        question.linear_start_level_txt = (row["linear_start_level_txt"] == DBNull.Value ? "" : row["linear_start_level_txt"].ToString());
                        question.linear_end_level_txt = (row["linear_end_level_txt"] == DBNull.Value ? "" : row["linear_end_level_txt"].ToString());

                        if (question.linear_end_value.HasValue && question.linear_end_value.Value > 0)
                        {
                            question.linearScale = question.linear_start_level_txt + " " + question.linear_start_value.Value.ToString() + ";" + question.linear_end_level_txt + " " + question.linear_end_value.Value.ToString();
                        }
                        break;
                    case "multiple-choice-grid":
                    case "checkbox-grid":
                        var ques_grid_columns = (row["ques_grid_columns"] == DBNull.Value ? null : row["ques_grid_columns"].ToString());
                        if (!string.IsNullOrWhiteSpace(ques_grid_columns))
                            assignQuesGridColumns(ques_grid_columns.Remove(ques_grid_columns.Length - 1, 1), ref question);
                        break;
                }
                questions.Add(question);
            }

            return questions;
        }


        public async Task<Question> GetQuestionOptions(int id, Question question)
        {
            try
            {
                DataTable dt = await _repo.GetQuestionOptions(id);
                List<QuestionOptions> questionOptions = new();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        QuestionOptions options = new();
                        options.id = Convert.ToInt32(dr["OPTIONS_ID"]);
                        options.name = dr["QUES_OPTIONS"] as string;
                        questionOptions.Add(options);
                    }
                }

                question.options = questionOptions.OrderBy(o => o.id).ToList();
                return question;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetQuestionOptions"));
            }
        }

        public async Task<Question> GetQuestionGridOptions(int id, Question question)
        {
            try
            {
                DataTable dt = await _repo.GetQuestionGridOptions(id);

                List<Column> columns = new();
                List<Row> rows = new();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var dr = dt.Rows[i];
                        Column column = new();
                        if (dr["COL_VAL"] != DBNull.Value)
                        {
                            column.id = Convert.ToInt32(dr["OPTIONS_ID"]);
                            column.name = dr["COL_VAL"] as string;
                        }

                        Row row = new();
                        if (dr["ROW_VAL"] != DBNull.Value)
                        {
                            row.name = dr["ROW_VAL"] as string;
                        }

                        if (!columns.Exists(c => c.name == column.name))
                            columns.Add(column);
                        if (!rows.Exists(r => r.name == row.name))
                            rows.Add(row);
                    }
                }

                question.columns = columns.OrderBy(o => o.id).ToList();
                question.rows = rows.ToList();

                var colNameList = question.columns.Select(c => c.name).Distinct().ToList();
                var rowNameList = question.rows.Select(c => c.name).Distinct().ToList();
                question.rowStr = string.Join(",", rowNameList);
                question.columnStr += string.Join(",", colNameList);
                return question;
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "GetQuestionGridOptions"));
            }
        }

        public async Task DeleteSurveyResponse(int surveyId, string retailerCode)
        {
            await _repo.DeleteSurveyResponse(surveyId, retailerCode);
        }

        public async Task<int> InsertSurveyResponse(List<SurveyResponse> model, string retailerCode, int userId)
        {
            return await _repo.InsertSurveyResponse(model, retailerCode, userId);
        }


        #region==========|  Private Method  |==========

        private void assignQuesOptions(string ques_options, ref Question question)
        {
            List<QuestionOptions> questionOptions = new();

            var optionDataArray = ques_options.Split('|');
            List<string> optionList = new();

            foreach (string optionData in optionDataArray)
            {
                var data = optionData.Split(',');

                QuestionOptions options = new();
                options.id = Convert.ToInt32(data[0].Substring(3));
                options.name = data[1].Substring(5);
                questionOptions.Add(options);
                optionList.Add(options.name);
            }

            question.options = questionOptions.OrderBy(o => o.id).ToList();
            question.optionsStr = string.Join(",", optionList);
        }


        private void assignQuesGridColumns(string ques_grid_columns, ref Question question)
        {
            List<Column> columns = new();
            List<Row> rows = new();

            var columnGridDataArray = ques_grid_columns.Split('|');
            string optStr = string.Empty;

            foreach (string columnGrid in columnGridDataArray)
            {
                var colData = columnGrid.Split(',');

                Column column = new();
                column.id = Convert.ToInt32(colData[0].Substring(3));
                column.name = colData[1].Substring(4);

                Row row = new();
                row.name = colData[2].Substring(4);

                if (!columns.Exists(c => c.name == column.name))
                    columns.Add(column);
                if (!rows.Exists(r => r.name == row.name))
                    rows.Add(row);
            }

            question.columns = columns.OrderBy(o => o.id).ToList();
            question.rows = rows;

            var colNameList = question.columns.Select(c => c.name).Distinct().ToList();
            var rowNameList = question.rows.Select(c => c.name).Distinct().ToList();
            question.rowStr = "Rows: " + string.Join(",", rowNameList);
            question.columnStr += "Columns: " + string.Join(",", colNameList);
        }



        #endregion==========|  Private Method  |==========

    }
}
