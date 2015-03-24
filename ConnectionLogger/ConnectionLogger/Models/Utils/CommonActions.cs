using ConnectionLogger.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace ConnectionLogger.Models.Utils
{
    /// <summary>
    /// Helper to allow navigation from viewmodels and other common things
    /// </summary>
    public static  class CommonActions
    {
        #region events
        public static event Action<string,string> ShowAllert;

        public static event Action<AppPages> OnNavigateToPage;
        #endregion

        #region Fields
        #endregion

        #region Properties
        
        #endregion

        #region Methods
        public static void DisplayAllert(string title, string message)
        {
            if (ShowAllert != null)
                ShowAllert(title, message);
        }

        public static void NavigateToPage(AppPages page)
        {
            if (OnNavigateToPage != null)
                OnNavigateToPage(page);
        }

        
        #endregion
    }
}
