using Domain.ResponseModel;

namespace Domain.Helpers
{
    public class SaveFileHelper
    {
        /// <summary>
        /// Save a file
        /// </summary>
        /// <param name="base64File"></param>
        /// <param name="retailerCode"></param>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        //public static string SaveBase64File(string base64File, string retailerCode, string fileLocation, string mime = null)
        //{
        //    try
        //    {
        //        string fileName = string.Empty;

        //        NetworkCredential networkCredential = new NetworkCredential(user, pass);

        //        using (new NetworkConnection(networkName, networkCredential))
        //        {
        //            Directory.CreateDirectory(fileLocation);

        //            var uniqueNumber = Guid.NewGuid().ToString().Substring(0, 6);

        //            var attatchType = GetFileExtension(base64File);
        //            fileName = uniqueNumber + retailerCode + "." + (string.IsNullOrEmpty(mime) ? attatchType.Extension : mime);

        //            string fullLocation = fileLocation + @"\" + fileName;

        //            File.WriteAllBytes(fullLocation, Convert.FromBase64String(base64File));

        //            return fileName;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        /// <summary>
        /// Save multiple file in one connection
        /// </summary>
        /// <param name="base64Files"></param>
        /// <param name="retailerCode"></param>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        //public static string SaveBase64Files(List<string> base64Files, string retailerCode, string fileLocation, string mime = null)
        //{
        //    try
        //    {
        //        string fileNameList = string.Empty;

        //        NetworkCredential networkCredential = new NetworkCredential(user, pass);

        //        using (new NetworkConnection(networkName, networkCredential))
        //        {
        //            Directory.CreateDirectory(fileLocation);

        //            for (int i = 0; i < base64Files.Count; i++)
        //            {
        //                var uniqueNumber = Guid.NewGuid().ToString().Substring(0, 6);

        //                string fileName = uniqueNumber + retailerCode + "." + "jpg";

        //                string fullLocation = fileLocation + @"\" + fileName;

        //                File.WriteAllBytes(fullLocation, Convert.FromBase64String(base64Files[i]));

        //                fileNameList += fileNameList.Length > 0 ? "," + fileName : fileName;
        //            }


        //            return fileNameList;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


        /// <summary>
        /// Get mime type from base64 string
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static FileExtensionModel GetFileExtension(string base64String)
        {
            string data = base64String.Substring(0, 5);

            return data.ToUpper() switch
            {
                "IVBOR" => new FileExtensionModel()
                {
                    MimeType = "image/png",
                    Extension = "png"
                },
                "/9J/4" => new FileExtensionModel()
                {
                    MimeType = "image/jpg",
                    Extension = "jpg"
                },
                "AAAAF" => new FileExtensionModel()
                {
                    MimeType = "video/mp4",
                    Extension = "mp4"
                },
                "JVBER" => new FileExtensionModel()
                {
                    MimeType = "application/pdf",
                    Extension = "pdf"
                },
                "AAABA" => new FileExtensionModel()
                {
                    MimeType = "image/x-icon",
                    Extension = "ico"
                },
                "UMFYI" => new FileExtensionModel()
                {
                    MimeType = "application/octet-stream",
                    Extension = "rar"
                },
                "E1XYD" => new FileExtensionModel()
                {
                    MimeType = "application/msword",
                    Extension = "rtf"
                },
                "RMLUZ" or "AHR0C" or "U1PKC" => new FileExtensionModel()
                {
                    MimeType = "text/plain",
                    Extension = "txt"
                },
                //case "MQOWM":
                //case "77U/M":
                //    //return "srt";
                //    return new
                //    {
                //        MimeType = "application/srt",
                //        Extension = "srt"
                //    };
                _ => new FileExtensionModel()
                {
                    MimeType = "image/jpg",
                    Extension = "jpg"
                },
            };
        }

    }
}