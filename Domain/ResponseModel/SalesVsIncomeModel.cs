using System.Data;

namespace Domain.ResponseModel
{
    public class SalesVsIncomeModel
    {
        /// <summary>
        /// Report Month
        /// </summary>
        public string month { get; set; }

        /// <summary>
        /// EV OTF commission + IRIS OTF commission, DataSource DWH
        /// </summary>
        public string otf { get; set; }

        /// <summary>
        /// Commission will be total of Product(Recharge, sim, sc) commsision and Post commission
        /// Post Commsission Include ETSAF Commission, GA(Gross Add), Reimbursement, Others
        /// </summary>
        public string commissionEarned { get; set; }

        public string commissionDisbursed { get; set; }

        public string ait { get; set; }

        public string otherCharge { get; set; }

        public string discount { get; set; }

        public string iTopUpSales { get; set; }

        public string scSales { get; set; }

        public string simSalesCount { get; set; }

        public string simSalesAmount { get; set; }

        public string incomeTotal { get; set; }

        public string salesTotal { get; set; }

        public string lastUpdatedOn { get; set; }

        public SalesVsIncomeModel(DataRow dr, string date)
        {
            if (dr.ItemArray.Length > 0)
            {
                month = date;
                otf = dr["OTF"] as string;
                commissionEarned = dr["COMMISSION_EARNED"] as string;
                commissionDisbursed = dr["COMMSSION_DISBURSED"] as string;
                ait = dr["AIT"] as string;
                otherCharge = dr["OTHER_CHARGE"] as string;
                discount = dr["DISCOUNT"] as string;
                iTopUpSales = dr["ITOPUP_SALES"] as string;
                scSales = dr["SC_SALES"] as string;
                simSalesCount = dr["SIM_SALES_COUNT"] as string;
                simSalesAmount = dr["SIM_SALES_AMOUNT"] as string;
                incomeTotal = dr["INCOME_TOTAL"] as string;
                salesTotal = dr["SALES_TOTAL"] as string;
                lastUpdatedOn = dr["LAST_UPDATED_ON"] as string;
            }
        }
    }
}
