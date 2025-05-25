///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Data;

namespace Domain.ResponseModel
{
    public class CommissionModel
    {
        public string retailerName { get; set; }

        public string retailerCode { get; set; }

        public string retailerMsisdn { get; set; }

        public string retailerAddress { get; set; }

        public string simSalesCount { get; set; }

        public string simReplacementCount { get; set; }

        public string rechargeAmount { get; set; }

        public string liftingSoldAmount { get; set; }

        public string total { get; set; }

        public string lastUpdatedOn { get; set; }

        public List<CommissionDetails> Items { get; set; }

        public CommissionModel(DataRow dr, List<CommissionDetails> items)
        {
            if (dr.ItemArray.Length > 0)
            {
                retailerName = dr["NAME"] as string;
                retailerCode = dr["CODE"] as string;
                retailerMsisdn = dr["MOBILENO"] as string;
                retailerAddress = dr["ADDRESS"] as string;
                simSalesCount = dr["TOTALSIMSALE"] as string;
                simReplacementCount = dr["TOTALSIMREPLACE"] as string;
                rechargeAmount = dr["TOTALRECHARGE"] as string;
                liftingSoldAmount = dr["TOTALLIFTING"] as string;
                total = dr["TOTAL"] as string;
                lastUpdatedOn = dr["LAST_UPDATED_ON"] as string;
                Items = items;
            }
        }
    }

    public class CommissionDetails
    {
        public string date { get; set; }

        public string description { get; set; }

        public string balance { get; set; }

        public string commissionEarned { get; set; }

        public string commissionDisbursed { get; set; }

        public string ait { get; set; }

        public string otherCharge { get; set; }

        public string totalBalance { get; set; }

        public CommissionDetails(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                date = dr["COMMISSIONDATE"] as string;
                description = dr["COMMISSIONNAME"] as string;
                commissionEarned = dr["COMMISSION"] as string;
                commissionDisbursed = dr["COMMDISBURSED"] as string;
                ait = dr["AIT"] as string;
                otherCharge = dr["OTHERCHARGES"] as string;
                balance = dr["BALANCE"] as string;
                totalBalance = dr["TOTALBALANCE"] as string;
            }
        }
    }

}