using System.Data;
using static Domain.Enums.EnumCollections;

namespace Domain.StaticClass
{
    public class NewNotificationsModelV2
    {
        public int id { get; set; }

        public string dateTime { get; set; }

        public string logoURL { get; set; }

        public string imageUrl { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string detailsUrl { get; set; }

        public string pdfUrl { get; set; }

        public bool isLiveTicker { get; set; }

        public string isRead { get; set; }

        public bool isReadOnly { get; set; }

        public string redirectToAction { get; set; }
        public string imageUrlLarge { get; set; }

        public ModelType modelType { get; set; }

        public bool isShortSurvey { get; set; }

        public string surveyType { get; set; }

        public string shortAnsType { get; set; }

        public string surveyWebLink { get; set; }


        public NewNotificationsModelV2(DataRow dr, string baseUrl)
        {
            if (dr.ItemArray.Length > 0)
            {
                if (dr["ID"] != DBNull.Value)
                {
                    id = Convert.ToInt32(dr["ID"].ToString());
                }

                dateTime = dr["NOTIFICATIONDATE"] as string;
                title = dr["TITLE"] as string;
                description = dr["DESCRIPTION"] as string;
                detailsUrl = dr["DETAILSURL"] as string;
                logoURL = dr["LOGOURL"] as string;
                isRead = Convert.ToString(dr["ISREAD"]);
                isReadOnly = DBNull.Value != dr["IS_READ_ONLY"] && Convert.ToBoolean(dr["IS_READ_ONLY"]);
                isLiveTicker = DBNull.Value != dr["ISLIVETICKER"] && Convert.ToBoolean(dr["ISLIVETICKER"]);
                modelType = (ModelType)Convert.ToInt32(dr["MODEL_TYPE"]);

                string _imgUrl = dr["IMAGEURL"] as string;
                string _imgUrlLarge = dr["IMAGEURL_LARGE"] as string;
                string _pdfUrl = dr["PDF_URL"] as string;

                if (!string.IsNullOrWhiteSpace(_imgUrl))
                {
                    imageUrl = baseUrl + _imgUrl;
                }

                if (!string.IsNullOrWhiteSpace(_imgUrlLarge))
                {
                    imageUrlLarge = baseUrl + _imgUrlLarge;
                }

                if (!string.IsNullOrWhiteSpace(_pdfUrl))
                {
                    pdfUrl = baseUrl + _pdfUrl;
                }

                if (dr.Table.Columns.Contains("REDIRECT_TO_ACTION"))
                {
                    redirectToAction = dr["REDIRECT_TO_ACTION"] as string;
                }

                if (modelType == ModelType.Survey)
                {
                    isShortSurvey = Convert.ToBoolean(dr["IS_SHORT_SURVEY"]);
                    surveyType = dr["SURVEY_TYPE"] as string;
                    shortAnsType = dr["ANSWER_TYPE"] as string;
                    surveyWebLink = dr["SURVEYWEBLINK"] as string;
                }
            }
        }

    }
}
