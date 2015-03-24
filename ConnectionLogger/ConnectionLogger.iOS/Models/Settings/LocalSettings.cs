using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace ConnectionLogger.Settings
{
    public class LocalSettings:ISettingsContainer
    {

        public LocalSettings()
        {
            InitLocalSettings();
        }

        XDocument _xmlProgress;

        
        private const string SettingsFile = "settings.xml";
        private readonly string SettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal); 


        private Dictionary<string, string> Values { get; set; }
        
        public string GetValue(string key)
        {
            if (Values == null)
                return null;
            lock (LockObj)
            {
                key = CheckKey(key);
                if (Values.ContainsKey(key))
                    return Values[key];
                return null;
            }
        }
        public void SetValue(string key, object value)
        {
            if (Values == null)
                InitLocalSettings();
            lock (LockObj)
            {
                key = CheckKey(key);
                if (Values.ContainsKey(key))
                    Values[key] = value.ToString();
                else
                    Values.Add(key, value.ToString());

                SaveProgress();
            }
            
        }

        public void SaveProgress()
        {
            if (_xmlProgress == null)
            {
                _xmlProgress = new XDocument();
                _xmlProgress.Add(new XElement("Settings"));
            }
            
            foreach (var key in Values.Keys)
            {
                try
                {
                    var pr = _xmlProgress.Descendants(key).FirstOrDefault();
                    var value = Values[key];
                    if (pr == null)
                    {
                        pr = new XElement(key, value);
                        _xmlProgress.Element("Settings").Add(pr);
                    }
                    else
                    {
                        pr.Value = value;
                    }
                }
                catch (Exception ex)
                {
                    var t = ex.Message;
                }
            }

            saveFile();
        }

        public void  InitLocalSettings()
        {
            if (Values != null)
                return;
            GetStorageFile();
            
        }

        public bool ContainsKey(string key)
        {
            return Values.ContainsKey(key);
        }
        
        static string CheckKey(string key)
        {
            key.Replace(" ", "_");
            return key;
        }

        readonly object LockObj = new object();

        void GetStorageFile()
        {
            try
            {
                Values = new Dictionary<string, string>();
                var fn = System.IO.Path.Combine(SettingsDir,SettingsFile); 

                 if (File.Exists(fn)) 
                 { 
                    using (var stream = File.Open(fn, FileMode.Open,FileAccess.Read)) 
                    { 

                        _xmlProgress = XDocument.Load(stream, LoadOptions.None);
                    }
                
                        if (_xmlProgress != null)
                        foreach (var el in _xmlProgress.Element("Settings").Elements())
                        {
                            SetValue(el.Name.LocalName, el.Value);
                        }
                    }
                }
            catch (Exception ex)
            {
                var t = ex.Message;
            }
            


        }

        
        
        void saveFile()
        {
            try
            {
                var fn = System.IO.Path.Combine(SettingsDir, SettingsFile);
                if (File.Exists(fn))
                    File.Delete(fn);

                using (var stream = File.Open(fn, FileMode.CreateNew, FileAccess.Write))
                {
                    _xmlProgress.Save(stream);
                }
            }
            catch(Exception ex)
            {
                var t = ex.Message;

            }
            
            
            
        }

 
        
    }
   
}
