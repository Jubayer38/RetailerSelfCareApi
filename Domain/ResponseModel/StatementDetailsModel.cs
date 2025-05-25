
///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Al Mamun
///	Purpose	      :	LoginSmartPos Controller
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
    public class StatementDetailsModel
    {
        public string date { get; set; }

        public string description { get; set; }

        public string liftingAmount { get; set; }

        public string salesAmount { get; set; }

        public string commission { get; set; }

        public string advanceIncomeTax { get; set; }

        public string amountReceived { get; set; }

        //public string total { get; set; }

        public StatementDetailsModel(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                date = dr["STATEMENTDATE"] as string;
                description = dr["DESCRIPTION"] as string;
                liftingAmount = dr["LIFTINGAMOUNT"] as string;
                salesAmount = dr["SALESAMOUNT"] as string;
                commission = dr["COMMISSION"] as string;
                advanceIncomeTax = dr["AIT"] as string;
                amountReceived = dr["AMOUNTRECEIVED"] as string;
            }
        }
    }
}
