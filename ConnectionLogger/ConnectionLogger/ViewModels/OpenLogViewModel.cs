using ConnectionLogger.Models.Utils;
using ConnectionLogger.Settings;
using Newtonsoft.Json.Linq;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;



namespace ConnectionLogger.ViewModels
{
    public class OpenLogViewModel:ParentViewModel
    {
        #region Constructor
        public OpenLogViewModel()
        {
            Title = "Open Log File...";
        }
        #endregion

        #region Events
        #endregion

        #region Members
        
        #endregion

        #region Properties
        
        #endregion

        #region Methods
        public async void RefreshLogs()
        {
            ClearChildren();
            var logs = await Global.GetLogs();
            foreach (var log in logs)
                AddChild(new LogFileViewModel(log),false);

            NotifyPropertyChanged("Children");
        }
        #endregion
    }
}
