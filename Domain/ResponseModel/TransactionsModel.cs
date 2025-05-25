///******************************************************************
///	|| Creation History ||
///--------------------------------------------------------------------
///	Copyright           :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	            :	Al Mamun
///	Purpose	            :	
///	Creation Date       :   16-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ------------------------------------------------------------------
///  Sl No.	Date:			Author:			Ver:	    Area of Change:
///  1.     28-Jan-2024		Basher Sarkar	7.0.0		add summary
///	 ----------------------------------------------------------------
///	*****************************************************************

using Domain.Helpers;
using Domain.RequestModel;
using Domain.ViewModel;
using System.Data;
using System.Globalization;

namespace Domain.ResponseModel
{
    public class TransactionsModel
    {
        public int id { get; set; }

        public string transactionNumber { get; set; }

        public string transactionType { get; set; }

        public string amount { get; set; }

        public string date { get; set; }

        public string msisdn { get; set; }

        public string commissionAmount { get; set; }

        public int inout { get; set; }

        public bool isOTF { get; set; }

        public TransactionsModel(DataRow dr)
        {
            if (dr["ID"] != DBNull.Value)
            {
                id = Convert.ToInt32(dr["ID"].ToString());
            }
            transactionNumber = dr["TRANNUMBER"] as string;
            transactionType = dr["TRANTYPE"] as string;
            amount = dr["AMOUNT"].ToString();
            date = dr["TRANDATE"] as string;
            msisdn = dr["MSISDN"] as string;
            commissionAmount = dr["COMMISSIONAMT"] as string;
            inout = dr["IN_OUT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["IN_OUT"]);
            isOTF = Convert.ToBoolean(dr["IS_OTF"]);
        }

        public TransactionsModel(C2CRechrgHistResp row)
        {
            transactionNumber = row.transactionNumber;
            transactionType = "EV Lifting";
            amount = (Convert.ToDouble(row.amount) * 100 / 97.25).ToString("0");
            date = DateTime.ParseExact(row.date, "dd/MM/yy HH:mm:ss", CultureInfo.InvariantCulture).ToEnUSDateString("hh:mm tt dd MMM yyyy");
            msisdn = row.msisdn;
            commissionAmount = "0";
            inout = 2;
            isOTF = false;
        }

        public static List<TransactionsModel> OrderingTransactionData(List<TransactionsModel> tranList, TransactionsRequest model)
        {
            List<TransactionsModel> finalTransactionDataList = [];

            if (!string.IsNullOrWhiteSpace(model.sortByAmt))
                finalTransactionDataList = model.sortByAmt.ToLower() == "asc" ?
                    tranList.OrderBy(o => HelperMethod.ParseNumberString(o.amount)).ToList() :
                    tranList.OrderByDescending(o => HelperMethod.ParseNumberString(o.amount)).ToList();
            if (!string.IsNullOrWhiteSpace(model.sortByInOut))
            {
                finalTransactionDataList = model.sortByInOut.ToLower() == "asc" ?
                    tranList.OrderBy(o => o.inout).ThenBy(o => o.amount).ToList() :
                    tranList.OrderByDescending(o => o.inout).ThenBy(o => o.amount).ToList();
            }
            if (!string.IsNullOrWhiteSpace(model.sortByDate))
                finalTransactionDataList = model.sortByDate.ToLower() == "asc" ?
                    tranList.OrderBy(o => Convert.ToDateTime(o.date)).ToList() :
                    tranList.OrderByDescending(o => Convert.ToDateTime(o.date)).ToList();

            return finalTransactionDataList;
        }

    }
}