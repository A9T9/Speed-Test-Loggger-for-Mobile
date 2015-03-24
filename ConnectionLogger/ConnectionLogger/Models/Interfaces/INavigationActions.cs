using System;
namespace ConnectionLogger
{
    public interface INavigationActions
    {
        event Action<string> OnNavigateToUrl;
        void NavigateToUrl(string url);
    }
}
