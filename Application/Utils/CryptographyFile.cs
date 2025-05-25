///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Cryptography Methods
///	Creation Date :	03-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Security.Cryptography;
using System.Text;

namespace Application.Utils
{
    public class CryptographyFile
    {
        public static string Encrypto(string value)
        {
            StringBuilder hash = new();
            byte[] bytes = MD5.HashData(new UTF8Encoding().GetBytes(value));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }


        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                keyArray = MD5.HashData(new UTF8Encoding().GetBytes("bl_smart_pos"));
            }
            else
                keyArray = Encoding.UTF8.GetBytes("bl_smart_pos");

            using TripleDES tripleDES = TripleDES.Create();
            //set the secret key for the tripleDES algorithm
            tripleDES.Key = keyArray;

            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tripleDES.Mode = CipherMode.ECB;

            //padding mode(if any extra byte added)
            tripleDES.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);

            //Release resources held by TripleDes Encryptor
            tripleDES.Clear();

            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray;
            //get the byte code of the string
            try
            {
                toEncryptArray = Convert.FromBase64String(cipherString);
            }
            catch (Exception)
            {

                throw new Exception("Invalid security token");
            }

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                keyArray = MD5.HashData(Encoding.UTF8.GetBytes("bl_smart_pos"));
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes("bl_smart_pos");
            }

            using TripleDES tripleDES = TripleDES.Create();

            //set the secret key for the tripleDES algorithm
            tripleDES.Key = keyArray;

            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)
            tripleDES.Mode = CipherMode.ECB;

            //padding mode(if any extra byte added)
            tripleDES.Padding = PaddingMode.PKCS7;

            byte[] resultArray;
            try
            {
                ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                resultArray = cTransform.TransformFinalBlock(
                                     toEncryptArray, 0, toEncryptArray.Length);
            }
            catch (Exception)
            {
                throw new Exception("Invalid security token");
            }

            //Release resources held by TripleDes Encryptor                
            tripleDES.Clear();

            //return the Clear decrypted TEXT
            return Encoding.UTF8.GetString(resultArray);
        }


        public static bool Verify(string stringValue, string encryptedValue)
        {
            // Hash the input.
            string hashOfInput = Encrypto(stringValue);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, encryptedValue))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}