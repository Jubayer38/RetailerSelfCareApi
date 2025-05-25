using System.Data;

namespace Domain.ResponseModel
{
    public class TarVsAchvSummaryModel
    {
        public string kpiId { get; set; }
        public string kpiName { get; set; }
        public string target { get; set; }
        public string achievement { get; set; }
        public string updateDate { get; set; }
        public string achievementPer { get; set; }
        public string RR { get; set; }
        public string CR { get; set; }
        public bool isAmount { get; set; }

        public TarVsAchvSummaryModel() { }

        public TarVsAchvSummaryModel(DataRow dr)
        {
            kpiId = dr["KPIID"] as string;
            kpiName = dr["KPINAME"] as string;
            target = dr["TARGET"] as string;
            achievement = dr["ACHV"] as string;
            updateDate = dr["ACHVDATE"] as string;
            achievementPer = dr["ACHVPER"] as string;
            RR = dr["RR"] as string;
            CR = dr["CR"] as string;
            isAmount = dr["ISAMOUNT"] != DBNull.Value ? Convert.ToBoolean(dr["ISAMOUNT"]) : false;
        }
    }
}
