///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Write text log
///	Creation Date :	18-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.StaticClass;
using System.Text;

namespace Domain.Helpers
{
    public class TextLogWriter
    {
        #region Global Parameters
        private static readonly ReaderWriterLockSlim locker = new();
        //private static readonly Mutex mutexLock = new Mutex();

        private const int NumberOfRetries = 3;
        // total time in milisecond
        private const int DelayOnRetry = 250;

        #endregion

        #region Write log into Text file

        public static void WriteApiLogToFile(StringBuilder logText, string path)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "ApiLogs", path);
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyyHH") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText.ToString());
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteApiTraceLogToFile(StringBuilder logText, string path)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "ApiLogs", path);
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyyHH") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText.ToString());
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteExternalApiLogToFile(StringBuilder logText, string path)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "ExternalApiLogs", path);
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteEVLogToFile(StringBuilder logText, string path)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "EVLogs", path);
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteIRISLogToFile(StringBuilder logText, string path)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "IRISLogs", path);
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteLogFromLogWriteError(string logText)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "TextWriteErrorLogs");
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteErrorLog(string logText)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "ErrorLogs");
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteRechargeExternalApiLogToFile(StringBuilder logText, string folderPrefix, string path)
        {
            string logFolderRootPath = TextLogging.TextLogPath;
            string logFolder = Path.Combine(logFolderRootPath, folderPrefix, path);
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteDmsApiLogToFile(StringBuilder logText, string path)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "DmsApiLogs", path);
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }


        public static void WriteRedisErrorLog(string logText)
        {
            Task.Factory.StartNew(() =>
            {
                RedisLogWriter(logText);
            });
        }


        private static void RedisLogWriter(string logText)
        {
            string logFolder = Path.Combine(TextLogging.TextLogPath, "RedisErrorLog");
            Directory.CreateDirectory(logFolder);

            string fileName = DateTime.Now.ToEnUSDateString("ddMMyyyy") + ".txt";
            string filePath = Path.Combine(logFolder, fileName);

            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();

            for (int i = 1; i <= NumberOfRetries; i++)
            {
                locker.EnterWriteLock();
                try
                {
                    using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter writer = new(fs);
                    writer.WriteLine(logText);
                    writer.Flush();
                    writer.Dispose();
                    fs.Close();

                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }

        #endregion

    }
}