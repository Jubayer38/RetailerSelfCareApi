
using Domain.Resources;
using Microsoft.Extensions.Localization;

///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Initialization for StringLocalizer
///	Creation Date :	01-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

namespace Domain.StaticClass
{
    public static class LocalizationFactoryService
    {
        private static IStringLocalizer<SharedResource> _stringLocalizer;

        public static void SetLocalization(IStringLocalizer<SharedResource> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public static IStringLocalizer<SharedResource> GetLocalizer()
        {
            return _stringLocalizer;
        }
    }
}
