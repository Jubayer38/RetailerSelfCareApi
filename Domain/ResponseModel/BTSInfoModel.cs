///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	15-Jan-2024
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
    public class BTSInfoModel
    {
        public int bTsId { get; set; }
        public string bts_code { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string bts_type { get; set; }
        public string technology_type { get; set; }
        public string sran { get; set; }
        public string thana { get; set; }
        public string district { get; set; }
        public string address { get; set; }

        public BTSInfoModel(DataRow dr)
        {
            if (dr.ItemArray.Length > 0)
            {
                bTsId = Convert.ToInt32((dr["BTS_ID"]));
                bts_code = dr["BTS_CODE"].ToString();
                bts_type = dr["BTS_TYPE"].ToString();
                technology_type = dr["TECHNOLOGY_TYPE"].ToString();
                latitude = Convert.ToDouble(dr["LATITUDE"]);
                longitude = Convert.ToDouble(dr["LONGITUDE"]);
                thana = (dr["THANA"]).ToString();
                district = dr["DISTRICT"].ToString();
                address = thana + ", " + district;
                sran = dr["SRAN"].ToString();
            }
        }
    }
}
