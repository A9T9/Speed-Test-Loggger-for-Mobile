
namespace ConnectionLogger.Settings
{
    public interface ISettingsContainer
    {
        string GetValue(string key);
        void SetValue(string key, object value);
    }
   
}
