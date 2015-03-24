#region Using
// Imported namespaces
using System;
using System.Collections.Generic;

using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Text;

using PCLStorage;
using System.Threading.Tasks;
using ConnectionLogger.Settings;
using System.Diagnostics;
#endregion
namespace ConnectionLogger
{
    /// <summary>The main class containing global objects and the entry point of the application.</summary>
    public static class Global
    {
        #region Members
        // Global variables
        private static List<ListViewItem> g_ConnectionLog=new List<ListViewItem>();

        #endregion

        #region Properties
        
        /// <summary>Gets the connection log associated with this application.</summary>
        public static List<ListViewItem> ConnectionLog
        {
            get { return g_ConnectionLog; }
        }

        /// <summary>Gets a value determining if this computer is currently connected to the network.</summary>
        public static bool IsOnline
        {
            get
            {
                return InetChecker.IsInternetAvailable();
            }
        }

        public static IInternetCheker InetChecker
        { get; set; }
                
        #endregion
        
        #region Public Methods
        /// <summary>Converts the specified bytes value into a human-readable string.</summary>
        public static string ConvertBytes(double bytes)
        {
            try
            {
                if (AppSettings.SpeedUnits)
                {
                    double num = bytes;
                    if (num > Math.Pow(1024.0, 4.0))
                    {
                        num /= Math.Pow(1024.0, 4.0);
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " TB");
                    }
                    if ((num > Math.Pow(1024.0, 3.0)) && (num < Math.Pow(1024.0, 4.0)))
                    {
                        num /= Math.Pow(1024.0, 3.0);
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " GB");
                    }
                    if ((num > Math.Pow(1024.0, 2.0)) && (num < Math.Pow(1024.0, 3.0)))
                    {
                        num /= Math.Pow(1024.0, 2.0);
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " MB");
                    }
                    if ((num > 1024.0) && (num < Math.Pow(1024.0, 2.0)))
                    {
                        num /= 1024.0;
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " KB");
                    }
                    if (num < 1024.0)
                    {
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " B");
                    }
                }
                else
                {
                    double num = bytes * 8;
                    if (num > Math.Pow(1024.0, 4.0))
                    {
                        num /= Math.Pow(1024.0, 4.0);
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " tb");
                    }
                    if ((num > Math.Pow(1024.0, 3.0)) && (num < Math.Pow(1024.0, 4.0)))
                    {
                        num /= Math.Pow(1024.0, 3.0);
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " gb");
                    }
                    if ((num > Math.Pow(1024.0, 2.0)) && (num < Math.Pow(1024.0, 3.0)))
                    {
                        num /= Math.Pow(1024.0, 2.0);
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " mb");
                    }
                    if ((num > 1024.0) && (num < Math.Pow(1024.0, 2.0)))
                    {
                        num /= 1024.0;
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " kb");
                    }
                    if (num < 1024.0)
                    {
                        return (Math.Round(num, 2).ToString("###,###,##0.00") + " b");
                    }
                }
            }
            catch
            {
                if (AppSettings.SpeedUnits) return "0 B";
                else return "0 b";
            }
            return null;
        }

        /// <summary>Converts the specified DateTime obejct into a human-readable string.</summary>
        public static string ConvertTime(DateTime time)
        {
            switch (AppSettings.TimeFormat)
            {
                case 0: // ISO Format
                    return time.ToString("yyyy/MM/dd H:mm:ss");
                case 1: // EU Format
                    return time.ToString("dd/MM/yyyy H:mm:ss");
                case 2: // US Format
                    return time.ToString("MM/dd/yyyy hh:mm:ss tt");
                default:
                    return time.ToString();
            }
        }
                
        /// <summary>Converts a Unix Timestamp value into a DateTime object.</summary>
        public static DateTime FromUnixTime(long time)
        {
            if (time == 0) return DateTime.Now;

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return epoch.AddSeconds(Convert.ToDouble(time)).ToLocalTime();
        }

        /// <summary>Loads the connection log from the specified CSV file.</summary>
        public async static Task LoadConnectionLog(string fileName)
        {
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("data",
                    CreationCollisionOption.OpenIfExists);

                if (await folder.CheckExistsAsync(fileName)== ExistenceCheckResult.FileExists)
                {

                    IFile file = await folder.GetFileAsync(fileName);
                                 
                    string lines = await file.ReadAllTextAsync();

                    using (StringReader sr = new StringReader(lines))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.ToLower().StartsWith("index"))
                            {
                                continue;
                            }

                            string[] s = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            if (s.Length > 6)
                            {
                                DateTime time = FromUnixTime(ToInt64(s[1]));
                                var speed = ToInt64(s[3]);
                                ListViewItem item = new ListViewItem(time,speed); // Time (String)
                                item.SubItems.Add(Global.ConvertBytes(speed)); // Speed (String)
                                item.SubItems.Add(s[5]); // IP
                                item.SubItems.Add(s[6]); // Message
                                item.SubItems.Add(s[7]); // Status
                                //item.SubItems.Add(s[4]); // Speed (Bytes)
                                item.ImageIndex = ToInt32(s[0]); // Index

                                g_ConnectionLog.Add(item);
                            }

                        }//while
                    }
                }         
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>Calculates a percentage (0.0-max%) given the specified values.</summary>
        public static float Percent(float complete, float total, float max = 100.0f)
        {
            if (complete <= 0.0f || total <= 0.0f) return 0.0f;

            return (complete / total) * max;
        }

        /// <summary>Calculates a percentage (0-max%) given the specified values.</summary>
        public static int Percent(int complete, int total, int max = 100)
        {
            if (complete == 0 || total == 0) return 0;

            double result = Convert.ToDouble(complete) / Convert.ToDouble(total);
            result *= Convert.ToDouble(max);

            return Convert.ToInt32(result);
        }

        /// <summary>Saves the connection log in CSV file format.</summary>
        public async static void SaveConnectionLog(string fileName)
        {
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("data",
                    CreationCollisionOption.OpenIfExists);
                IFile file = await folder.CreateFileAsync(fileName,
                    CreationCollisionOption.ReplaceExisting);

                StringBuilder writer = new StringBuilder();
                
                writer.AppendLine("Index,Time(U),Time(S),Speed(B),Speed(S),IP,Message,Status");

                foreach (ListViewItem item in g_ConnectionLog)
                {
                    DateTime time = item.Time;

                    writer.AppendLine(item.ImageIndex.ToString() + "," + // Index
                        ToUnixTime(time).ToString() + "," + // Time (Unix Timestamp)
                        ConvertTime(time) + "," + // Time (String)
                        item.SpeedByte + "," + // Speed (Bytes)
                        item.SubItems[1].Replace(",",".") + "," + // Speed (String)
                        item.SubItems[2] + "," + // IP
                        item.SubItems[3] + "," + // Message
                        item.SubItems[4]); // Status
                }
                var s = writer.ToString();
                await file.WriteAllTextAsync(writer.ToString());
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>Calculates a scalar value (0.0-1.0) given the specified values.</summary>
        public static float Scalar(float complete, float total)
        {
            if (complete <= 0.0f || total <= 0.0f) return 0.0f;

            return complete / total;
        }

        /// <summary>Converts a string value into a Int32 value.</summary>
        public static int ToInt32(string s)
        {
            int result = 0;

            if (Int32.TryParse(s, out result)) return result;
            return 0;
        }

        /// <summary>Converts a string value into a Int64 value.</summary>
        public static long ToInt64(string s)
        {
            long result = 0;

            if (Int64.TryParse(s, out result)) return result;
            return 0;
        }

        /// <summary>Converts a DateTime object into a Unix Timestamp value.</summary>
        public static long ToUnixTime(DateTime time)
        {
            return Convert.ToInt64((time - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds);
        }

        /// <summary>
        /// Get names of all log files 
        /// </summary>
        public static async Task<List<string>> GetLogs()
        {
            var rv = new List<string>();
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("data",
                    CreationCollisionOption.OpenIfExists);

                var files = await folder.GetFilesAsync();
                foreach (var file in files)
                { rv.Add(file.Name); }
            }
            catch { }
            return rv;
        }
        #endregion
    }
}
