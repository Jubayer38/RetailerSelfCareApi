///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Enum Collections
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.ComponentModel;

namespace Domain.Enums
{
    public class EnumCollections
    {
        public enum NotificationType
        {
            Ticker = 1, Notification, FlashPopUp
        }

        public enum OfferType
        {
            RechargeOffer = 1,
            DenoOffer = 2
        }

        public enum ModelType
        {
            Ticker = 1, Notification, FlashPopUp, Survey
        }

        public enum FrequentlyUsedDbParams
        {
            P_PKVALUE, P_RETURN
        }

        public enum LmsAdjustmentType
        {
            CREDIT, DEBIT
        }

        public enum LmsTermsFaq
        {
            TermsConditions = 1, Faq
        }

        public enum APIVersoinEnum : int
        {
            None = 0,
            Old = 1,
            New = 2
        }


        public enum ChangePasswordEnum : int
        {
            invalidUser = -888,
            passwordNotMatched = -777,
            unableToUpdate = -999
        }


        public enum EnumForgetPWDStatus
        {
            Success = 1,
            SMSSendFailed = -777,
            UpdateFailed = -888,
            UserInfoNotFound = -1111
        }


        public enum EnumSecurityTokenPropertyIndex
        {
            LoginProvider = 0,
            uid = 1,
            unamen = 2,
            dc = 3,//DistributorCode
            deviceId = 4
        }


        public enum EnumValidateOrder
        {
            ValidOrder = 1,
            MsisdnOnProcess = -111,
            SIMOnProcess = -222,
            OnProcess = -333,
            DBFailed = -999
        }

        public enum RechargeType
        {
            EvRecharge = 1,
            IrisRecharge
        }

        public enum PaymentType : int
        {
            prepaid = 1,
            postpaid = 2
        }

        public enum OrderItemsRso
        {
            SC = 1, SIM, EV
        }

        public enum OrderItemsApp
        {
            [Description("Scratch card")]
            SC = 1,
            [Description("Sim")]
            SIM,
            [Description("iTopUP")]
            ITOPUP
        }

        public enum OrderStatus
        {
            Pending, Delivered, Not_Delivered, Partial_Delivered, Failed = -1
        }

    }
}