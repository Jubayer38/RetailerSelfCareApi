///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	14-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Data;


namespace Domain.ResponseModel
{
    public class OperatorsModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public OperatorsModel(DataRow dr)
        {
            if (dr.ItemArray.Count() > 0)
            {
                Id = dr["OP_ID"] as string;
                Name = dr["OP_NAME"] as string;
            }
        }
    }
}
