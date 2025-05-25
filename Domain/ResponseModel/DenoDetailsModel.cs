using Domain.Helpers;
using System.Data;

namespace Domain.ResponseModel
{
    public class DenoDetailsModel
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int mtdTranCount { get; set; }
        public int mtdTranAmount { get; set; }
        public int yesterdayTranCount { get; set; }
        public int yesterdayTranAmount { get; set; }
        public int mtdTillPrvDayTranCount { get; set; }
        public int mtdTillPrvDayTranAmount { get; set; }
        public int lmTranCount { get; set; }
        public int lmTranAmount { get; set; }


        public DenoDetailsModel()
        {
            DateTime today = DateTime.Today;
            DateTime _date = new(today.Year, today.Month, 1);

            fromDate = _date.ToEnUSDateString("dd MMM yyyy");
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            toDate = new DateTime(today.Year, today.Month, daysInMonth).ToEnUSDateString("dd MMM yyyy");
        }


        public DenoDetailsModel(DataRow row)
        {
            DateTime today = DateTime.Today;
            DateTime _date = new(today.Year, today.Month, 1);

            fromDate = _date.ToEnUSDateString("dd MMM yyyy");
            int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);
            toDate = new DateTime(today.Year, today.Month, daysInMonth).ToEnUSDateString("dd MMM yyyy");

            if (row.ItemArray.Length > 0)
            {
                fromDate = row["START_DATE"] as string;
                toDate = row["END_DATE"] as string;
                mtdTranCount = row["TRAN_COUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["TRAN_COUNT"].ToString());
                mtdTranAmount = row["TRAN_AMOUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["TRAN_AMOUNT"].ToString());
                yesterdayTranCount = row["Y_TXN_COUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["Y_TXN_COUNT"].ToString());
                yesterdayTranAmount = row["Y_TRAN_AMOUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["Y_TRAN_AMOUNT"].ToString());
                mtdTillPrvDayTranCount = row["MTD_TXN_COUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["MTD_TXN_COUNT"].ToString());
                mtdTillPrvDayTranAmount = row["MTD_TXN_AMOUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["MTD_TXN_AMOUNT"].ToString());
                lmTranCount = row["LM_TXN_COUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["LM_TXN_COUNT"].ToString());
                lmTranAmount = row["LM_TRAN_AMOUNT"] == DBNull.Value ? 0 : Convert.ToInt32(row["LM_TRAN_AMOUNT"].ToString());
            }
        }
    }
}
