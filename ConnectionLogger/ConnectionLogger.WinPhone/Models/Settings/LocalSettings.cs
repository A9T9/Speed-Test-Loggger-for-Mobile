using System.IO.IsolatedStorage;

namespace ConnectionLogger.Settings
{
    public class LocalSettings:ISettingsContainer
    {
        
        public string GetValue(string key)
        {
            key = CheckKey(key);
            return IsolatedStorageSettings.ApplicationSettings.Contains(key) ? IsolatedStorageSettings.ApplicationSettings[key].ToString() : null;
        }

        public void SetValue(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        
        string CheckKey(string key)
        {
            key=key.Replace(" ", "_");
            return key;
        }

        public static void InitLocalSettings() { }
        
    }
   
}
