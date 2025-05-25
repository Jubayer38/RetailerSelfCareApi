///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Message collection for Retailer Selfcare app
///	Creation Date :	02-Jan-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Diagnostics;

namespace Domain.StaticClass
{
    public static class Message
    {
        public static string Success { get { return "Success"; } }
        public static string SaveSuccess { get { return "Save Successful"; } }
        public static string UpdateSuccess { get { return "Update Successful"; } }
        public static string Failed { get { return "Failed"; } }
        public static string FailedSteps { get { return "One or more operation failed"; } }
        public static string SomethingWentWrong { get { return "Something went wrong!!"; } }
        public static string BadRequest { get { return "Bad Request"; } }
        public static string InvalidSession { get { return "Invalid session token , Please Login Again"; } }
        public static string InvalidOperationType { get { return "Invalid operation type"; } }
        public static string LogoutSuccess { get { return "Successfully logged out from this device."; } }
        public static string LogoutFailed { get { return "Unable to logout from this device."; } }
        public static string MailSentSuccessful { get { return "Mail Sent Successfully"; } }
        public static string ReportNotMatched { get { return "Report type not matched"; } }
        public static string InvalidPin { get { return "Invalid Pin"; } }
        public static string NothingToExecute { get { return "Nothing to execute"; } }

        public static string DeviceDeregistered { get { return "This device is De-Registered."; } }
        public static string DeviceDisable { get { return "This device is not Enable."; } }

        public static string RegisterFailed { get { return "Unable to register this device."; } }
        public static string DeviceValidFailed { get { return "Device validation failed."; } }
        public static string UnableToSaveResponse { get { return "Unable to save response."; } }
        public static string FeedbackSavedSuccessfully { get { return "Your feedback saved successfully."; } }
        public static string FeedbackNotSaved { get { return "Can't save your feedback."; } }
        public static string YourVoiceSavedSuccessfully { get { return "Your voice saved successfully."; } }
        public static string VoiceNotSaved { get { return "Can't save your voice."; } }
        public static string CampaignEnrollSuccessful { get { return "Campaign Enroll Successful."; } }
        public static string CampaingKPIOverlap { get { return "Please try again after the enrolled campaign ends."; } }
        public static string CancelCampaignEnrollmentSuccessful { get { return "Cancel Campaign Enrollment Successful."; } }
        public static string YouAlreadyHaveEnrolledForMaximumTimeForThisCampaign { get { return "You already have enrolled for maximum time for this campaign."; } }
        public static string PDFFileAttachAsBase64String { get { return "PDF file attach as Base64 string."; } }
        public static string DeviceNotEnabled { get { return "Device must be enabled to make primary."; } }
        public static string ReceiptSendViaEmail { get { return "Receipt send via e-mail."; } }
        public static string ContactSaveFailed { get { return "Failed to save contact"; } }

        public static string ContactExist { get { return "This contact already exist"; } }
        public static string InvalidLogedIn { get { return "User Name or Password Invalid!"; } }
        public static string SubmitDigitalServiceSuccessful { get { return "Your request successfully submitted"; } }
        public static string NewDeviceRequestSuccess { get { return "Your request successfully submitted."; } }
        public static string NewDeviceRequestFailed { get { return "Unable to submit request. Please try again."; } }
        public static string NewDeviceRequestPending { get { return "You already have one pending request. Please contact with your RSO."; } }

        public static string SCSalesSuccess { get { return "Scratch Card Sold Successfully."; } }
        public static string SCSalesFailed { get { return "Unable to sale this Scratch Card. Please try again!"; } }
        public static string NoResponse { get { return "Recharge Request sent empty response."; } }

        public static string NoRsoFound { get { return "Your're not assign with any RSO."; } }
        public static string MultipleAmarOfferAvailable { get { return "Multiple Amar Offer found by the same amount."; } }
        public static string InvalidAmarOffer { get { return "Invalid Amar Offer!"; } }
        public static string AmarOfferSmsSuccess { get { return "Successfully sms sent to Customer."; } }

        public static string ExceptionDetails(Exception ex)
        {
            return "Message: " + ex.Message + " Method Name:" + new StackTrace(ex).GetFrame(0).GetMethod().Name + " StackTrace:" + ex.StackTrace + " Source:" + ex.Source;
        }

        public static string EMailBody(decimal amount, string retailerNo, string tran)
        {
            return "<p> Successfully Tk " + amount.ToString() + " recharged. Retailer: " + retailerNo + ". Tran: " + tran + "</p>";
        }

        public static string NoDataFound { get { return "No data found!"; } }
        public static string NoDataFoundForMail { get { return "No data found for Sending Mail!"; } }
        public static string MSISDNInvalid { get { return "MSISDN is invalid!"; } }
        public static string MSISDNValid { get { return "MSISDN is valid!"; } }
        public static string OTPFailed { get { return "OTP generation failed!"; } }
        public static string InvalidOTP { get { return "Invalid OTP!"; } }
        public static string ValidOTP { get { return "OTP is valid!"; } }
        public static string DBError { get { return "Database operation failed!"; } }
        public static string SMSSendFailed { get { return "SMS send failed!"; } }
        public static string UpdateFailed { get { return "Update failed!!"; } }
        public static string OrderSubmitSuccessfull { get { return "Order submitted successfully."; } }
        public static string StockIDMismatch { get { return "You are not authorized for this connection!"; } }
        public static string InvalidAttempt { get { return "Invalid attempt"; } }
        public static string PleaseTryAgain { get { return "Please try again."; } }
        public static string OrderAlreadyInProcess { get { return "This order is already in process."; } }
        public static string OrderCreationFaild { get { return "Unable to generate request token."; } }
        public static string OTPSentToMobile { get { return "OTP has been sent to your mobile number."; } }
        public static string RequestTimeOut { get { return "Request Timeout."; } }
        public static string MissingSOCategory { get { return "Missing Category Information"; } }
        public static string ComplaintSubmitSuccessful { get { return "Your Complaint Successfully Submitted."; } }
        public static string ComplaintSubmitFailed { get { return "Complaint Submission Failed. Please Submit Again."; } }
        public static string CanNotRaiseToSO { get { return "Could not raise the complaint to Super Office."; } }
        public static string RsoOrZmInfoNotFound { get { return "{0} Information not found."; } }
        public static string LMSRewardCouldNotRedeem { get { return "Could Not Redeem Reward"; } }
        public static string DeleteFailed { get { return "Unable to delete data."; } }
        public static string NoAmarOfferAvailable { get { return "No Amar Offers Found."; } }
        public static string NoRsoDataFound { get { return "No RSO Data found"; } }
    }
}