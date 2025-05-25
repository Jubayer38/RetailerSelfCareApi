///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	DD-MMM-YYYY
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.Helpers;
using System.Data;


namespace Domain.ResponseModel
{
    public class SalesVsCommissionModel
    {
        public string lastUpdatedOn { get; set; }
        public double totalSales { get; set; }
        public double totalCommission { get; set; }
        public double totalSIMCommission { get; set; }
        public double simUpfront { get; set; }
        public double activationCommission { get; set; }
        public double frCommission { get; set; }
        public double compliance { get; set; }
        public double totalITopUpCommission { get; set; }
        public double amarOffer { get; set; }
        public double regularCommission { get; set; }
        public double iTopUpUpfront { get; set; }
        public double totalCampaignCommission { get; set; }
        public double totalITopUpSales { get; set; }
        public int totalSimSalesCount { get; set; }


        public SalesVsCommissionModel(DataTable dataTable, string filterText)
        {
            if (dataTable.Rows.Count > 0)
            {
                DataRow row = dataTable.Rows[0];

                lastUpdatedOn = row["LAST_UPDATED_ON"].ToString();

                _ = double.TryParse(row["TOTAL_SALES"].ToString(), out double _totalSales);
                totalSales = _totalSales;

                _ = double.TryParse(row["TOTAL_COMMISSION"].ToString(), out double _totalCommission);
                totalCommission = _totalCommission;

                _ = double.TryParse(row["TOTAL_SIM_COMMISSION"].ToString(), out double _totalSimCommission);
                totalSIMCommission = _totalSimCommission;

                _ = double.TryParse(row["SIM_UPFRONT"].ToString(), out double _simUpfront);
                simUpfront = _simUpfront;

                _ = double.TryParse(row["ACTIVATION_COMMISSION"].ToString(), out double _activationCommission);
                activationCommission = _activationCommission;

                _ = double.TryParse(row["FR_COMMISSION"].ToString(), out double _frCommission);
                frCommission = _frCommission;

                _ = double.TryParse(row["COMPLIANCE_AMOUNT"].ToString(), out double _complianceAmt);
                compliance = _complianceAmt;

                _ = double.TryParse(row["TOTAL_ITOPUP_COMMISSION"].ToString(), out double _totalITopupCommission);
                totalITopUpCommission = _totalITopupCommission;

                _ = double.TryParse(row["AMAR_OFFER_COMMISSION"].ToString(), out double _amarOfferCommission);
                amarOffer = _amarOfferCommission;

                _ = double.TryParse(row["REGULAR_COMMISSION"].ToString(), out double _regularCommission);
                regularCommission = _regularCommission;

                _ = double.TryParse(row["ITOPUP_UPFRONT"].ToString(), out double _itpupUpfront);
                iTopUpUpfront = _itpupUpfront;

                _ = double.TryParse(row["CAMPAIGN_COMMISSION"].ToString(), out double _campCommission);
                totalCampaignCommission = _campCommission;

                _ = double.TryParse(row["ITOPUP_SALES"].ToString(), out double _itopupSales);
                totalITopUpSales = _itopupSales;

                _ = int.TryParse(row["SIM_SALES_COUNT"].ToString(), out int _simSalesCount);
                totalSimSalesCount = _simSalesCount;
            }
            else
            {
                DateTime todayDate = DateTime.Now;
                int year = todayDate.Year; int month = todayDate.Month; int day = todayDate.Day;

                if (filterText.Equals("thismonth", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime dateTime = new(year, month, day, 0, 0, 0);
                    lastUpdatedOn = dateTime.ToEnUSDateString("hh:mm tt, dd MMM yyyy");
                }
                else
                {
                    month = month == 1 ? 12 : month - 1;
                    year = month == 12 ? year - 1 : year;
                    int prvMonthLastDay = DateTime.DaysInMonth(year, month);
                    DateTime dateTime = new(year, month, prvMonthLastDay, 0, 0, 0);
                    lastUpdatedOn = dateTime.ToEnUSDateString("hh:mm tt, dd MMM yyyy");
                }
            }
        }
    }
}
