///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	User Validation Response Model
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.ResponseModel
{
    public class LoginUserInfoResponse
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string role_id { get; set; }
        public string role_name { get; set; }
        public int? is_role_active { get; set; }
        public int? channel_id { get; set; }
        public string channel_name { get; set; }
        public int? is_activedirectory_user { get; set; }
        public string role_access { get; set; }
        public string distributor_code { get; set; }
        public int inventory_id { get; set; }
        public string center_code { get; set; }
    }


    public class LoginUserInfoResponseV2 : LoginUserInfoResponse
    {
        public string regionCode { get; set; }
        public string regionName { get; set; }
    }
}