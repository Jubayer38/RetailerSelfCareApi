using System.Data;
using System.Text.Json.Serialization;

namespace Domain.ResponseModel
{
    public class QuickAccessModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string bengaliTitle { get; set; }
        public string iconBase64 { get; set; }

        [JsonIgnore]
        public string iconBase64Dark { get; set; }
        public string redirectUrl { get; set; }
        public string packageName { get; set; }
        public string appAction { get; set; }
        public string appType { get; set; }
        public bool isWidgetActive { get; set; }
        public bool hasRight { get; set; }


        public QuickAccessModel() { }

        public QuickAccessModel(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                id = Convert.ToInt32(dr["APP_ID"]).ToString();
                title = dr["APP_TITLE"] as string;
                bengaliTitle = dr["APP_TITLE_BN"] as string;
                redirectUrl = dr["APP_REDIRECT_URL"] as string;
                packageName = dr["APP_PACKAGE_NAME"] as string;
                appAction = dr["APP_ACTION"] as string;
                appType = dr["APP_TYPE"] as string;
                isWidgetActive = Convert.ToBoolean(dr["IS_ACTIVE_IN_QA"]);
                hasRight = true;

                iconBase64 = dr["ICON_BASE64"] as string;
            }
        }


        public QuickAccessModel SetIcon(bool isDark)
        {
            iconBase64 = (isDark && iconBase64Dark != null) ? iconBase64Dark : iconBase64;

            return this;
        }

    }
}
