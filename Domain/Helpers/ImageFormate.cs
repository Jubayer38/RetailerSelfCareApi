///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Directory browsing contains http [baseUrl]. get file from direct path form another server have pass network credential
///	Creation Date :	15-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using System.Net;

namespace Domain.Helpers
{
    public static class ImageFormate
    {
        public static KeyValuePair<string, string> UrlToBase64(string path, string baseUrl)
        {
            try
            {
                string BaseUrl = path.Contains("http", StringComparison.CurrentCultureIgnoreCase) ? "" : baseUrl;
                path = BaseUrl + path;
                byte[] bytes;
                string base64;

                if (!baseUrl.Contains("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    bytes = File.ReadAllBytes(path);
                    base64 = Convert.ToBase64String(bytes);
                }
                else
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    WebClient client = new();
                    Stream stream = client.OpenRead(path);

                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    base64 = Convert.ToBase64String(bytes);
                    stream.Flush();
                    stream.Close();
                    client.Dispose();
                }

                return new KeyValuePair<string, string>(path, base64);
            }
            catch (Exception)
            {
                path = "[" + path + "]";
                string base64 = @"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";
                return new KeyValuePair<string, string>(path, base64);
            }
        }


        #region=======================|     Retailer App CR    |========================

        public static KeyValuePair<string, string> UrlToBase64V2(string path, string baseUrl)
        {
            try
            {
                string BaseUrl = path.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? "" : baseUrl;
                path = BaseUrl + path;
                byte[] bytes;
                string base64;

                if (!baseUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    bytes = File.ReadAllBytes(path);
                    base64 = Convert.ToBase64String(bytes);
                }
                else
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using WebClient client = new();
                    bytes = client.DownloadData(path);
                    base64 = Convert.ToBase64String(bytes);

                    client.Dispose();
                }

                return new KeyValuePair<string, string>(path, base64);
            }
            catch (Exception)
            {
                path = "[" + path + "]";
                string base64 = @"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";

                return new KeyValuePair<string, string>(path, base64);
            }
        }


        /// <summary>
        /// Using Httpclient
        /// </summary>
        /// <param name="path"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> UrlToBase64V3(string path, string baseUrl)
        {
            try
            {
                string BaseUrl = path.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? "" : baseUrl;
                path = BaseUrl + path;
                byte[] bytes;
                string base64;

                if (!baseUrl.StartsWith("http", StringComparison.CurrentCultureIgnoreCase))
                {
                    bytes = File.ReadAllBytes(path);
                    base64 = Convert.ToBase64String(bytes);
                }
                else
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    using HttpClient client = new();
                    bytes = client.GetByteArrayAsync(path).GetAwaiter().GetResult();
                    base64 = Convert.ToBase64String(bytes);

                    client.Dispose();
                }

                return new KeyValuePair<string, string>(path, base64);
            }
            catch (Exception)
            {
                path = "[" + path + "]";
                string base64 = @"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";

                return new KeyValuePair<string, string>(path, base64);
            }
        }

        #endregion

    }
}