using System.Data;

namespace Domain.ResponseModel
{
    public class StatementMasterModel
    {
        public string retailerName { get; set; }

        public string retailerCode { get; set; }

        public string retailerMsisdn { get; set; }

        public string retailerAddress { get; set; }

        public string month { get; set; }

        public string previousBalance { get; set; }

        public string rechargeLifting { get; set; }

        public string simLifting { get; set; }

        public string scLifting { get; set; }

        public string totalCommission { get; set; }

        public string loyaltiPointEarned { get; set; }

        public string loyaltiPointRedeemed { get; set; }

        public string availableLoyaltiPoint { get; set; }

        public string closingBalance { get; set; }

        public string total { get; set; }

        public string lastUpdatedOn { get; set; }

        public List<StatementDetailsModel> statementDetails { get; set; }


        public StatementMasterModel(DataRow dr, List<StatementDetailsModel> items)
        {
            if (dr.ItemArray.Length > 0)
            {
                retailerName = dr["NAME"] as string;
                retailerCode = dr["CODE"] as string;
                retailerMsisdn = dr["MOBILENO"] as string;
                retailerAddress = dr["ADDRESS"] as string;
                previousBalance = dr["PRVBALANCE"] as string;
                rechargeLifting = dr["RECHARGE_LIFTING"] as string;
                simLifting = dr["SIM_LIFTING"] as string;
                scLifting = dr["SC_LIFTING"] as string;
                totalCommission = dr["TOTAL_COMMISSION"] as string;
                loyaltiPointEarned = dr["TOTAL_LP_EARNED"] as string;
                loyaltiPointRedeemed = dr["TOTAL_LP_REDEEMED"] as string;
                availableLoyaltiPoint = dr["TOTAL_AVAILABLE_LP"] as string;
                closingBalance = dr["CLOSING_BALANCE"] as string;
                lastUpdatedOn = dr["LAST_UPDATED_ON"] as string;
                total = dr["TOTAL"] as string;

                statementDetails = items;
            }
        }

    }
}
