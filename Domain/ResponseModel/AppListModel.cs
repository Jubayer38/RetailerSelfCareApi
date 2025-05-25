using System.Data;

namespace Domain.ResponseModel
{
    public class AppListModel
    {
        public string appName { get; set; }
        public string appVersion { get; set; }
        public string url { get; set; }
        public string packageName { get; set; }
        public string iconBase64 { get; set; }


        public AppListModel(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                appName = dr["APP_NAME"] as string;
                appVersion = dr["APP_VERSION"] as string;
                url = dr["URL"] as string;
                packageName = dr["APP_PACKAGE_NAME"] as string;
                iconBase64 = dr["ICON_BASE64"] as string;
            }
        }
    }
}
