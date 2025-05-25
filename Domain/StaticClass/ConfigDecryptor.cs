
namespace Domain.StaticClass
{
    public class ConfigDecryptor
    {
        public static void DecryptConnectionString()
        {
            Connections.DMSCS = Cryptography.Decryptor(Connections.DMSCS);
            Connections.RetAppDbCS = Cryptography.Decryptor(Connections.RetAppDbCS);
            Connections.REDISCS = Cryptography.Decryptor(Connections.REDISCS);
            Connections.RetAppMySqlCS = Cryptography.Decryptor(Connections.RetAppMySqlCS);
        }


        public static void DecryptResponseMessages()
        {
            ResponseMessages.InvalidSessionMsg = Cryptography.Decryptor(ResponseMessages.InvalidSessionMsg);
            ResponseMessages.ValidSessionMsg = Cryptography.Decryptor(ResponseMessages.ValidSessionMsg);
            ResponseMessages.InvalidUserCred = Cryptography.Decryptor(ResponseMessages.InvalidUserCred);
            ResponseMessages.CredPSentToMobile = Cryptography.Decryptor(ResponseMessages.CredPSentToMobile);
            ResponseMessages.InvalidCredP = Cryptography.Decryptor(ResponseMessages.InvalidCredP);
        }


        public static void DeecryptExternalKeysString()
        {
            ExternalKeys.EvURL = Cryptography.Decryptor(ExternalKeys.EvURL);
            ExternalKeys.SMSApiUrl = Cryptography.Decryptor(ExternalKeys.SMSApiUrl);
            ExternalKeys.IrisUsername = Cryptography.Decryptor(ExternalKeys.IrisUsername);
            ExternalKeys.IrisCred = Cryptography.Decryptor(ExternalKeys.IrisCred);
            ExternalKeys.Irischannel = Cryptography.Decryptor(ExternalKeys.Irischannel);
            ExternalKeys.IrisGatewayCode = Cryptography.Decryptor(ExternalKeys.IrisGatewayCode);
            ExternalKeys.EvPinLessBlncURL = Cryptography.Decryptor(ExternalKeys.EvPinLessBlncURL);
            ExternalKeys.POSMQKey = Cryptography.Decryptor(ExternalKeys.POSMQKey);
            ExternalKeys.RsoUser = Cryptography.Decryptor(ExternalKeys.RsoUser);
            ExternalKeys.RsoCred = Cryptography.Decryptor(ExternalKeys.RsoCred);
            ExternalKeys.InternalUser = Cryptography.Decryptor(ExternalKeys.InternalUser);
            ExternalKeys.EvPinChangeUrl = Cryptography.Decryptor(ExternalKeys.EvPinChangeUrl);
            ExternalKeys.SuperOfficeUserName = Cryptography.Decryptor(ExternalKeys.SuperOfficeUserName);
            ExternalKeys.SuperOfficeCred = Cryptography.Decryptor(ExternalKeys.SuperOfficeCred);
            ExternalKeys.SuperOfficeInternalUser = Cryptography.Decryptor(ExternalKeys.SuperOfficeInternalUser);
            ExternalKeys.SuperOfficeInternalCred = Cryptography.Decryptor(ExternalKeys.SuperOfficeInternalCred);
            ExternalKeys.SMS_Send_Url = Cryptography.Decryptor(ExternalKeys.SMS_Send_Url);
            ExternalKeys.RetailerApiToWebCred = Cryptography.Decryptor(ExternalKeys.RetailerApiToWebCred);
        }


        public static void DecryptBiometricKeysString()
        {
            BiometricKeys.BioRetailerStatusUserName = Cryptography.Decryptor(BiometricKeys.BioRetailerStatusUserName);
            BiometricKeys.BioRetailerStatusCred = Cryptography.Decryptor(BiometricKeys.BioRetailerStatusCred);
        }


        public static void DecryptLMSKyesString()
        {
            LMSKyes.LmsChannel = Cryptography.Decryptor(LMSKyes.LmsChannel);
        }


        public static void DecryptEmailKeysString()
        {
            EmailKeys.SenderEmail = Cryptography.Decryptor(EmailKeys.SenderEmail);
            EmailKeys.SenderCred = Cryptography.Decryptor(EmailKeys.SenderCred);
        }

    }
}