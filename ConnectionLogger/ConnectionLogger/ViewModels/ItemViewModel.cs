using ConnectionLogger.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConnectionLogger.ViewModels
{
    class ItemViewModel:BaseViewModel
    {
        
        public ItemViewModel(ListViewItem item)
        {
            _item = item;
            if (item != null && item.SubItems.Any())
                Title = item.SubItems[0];
            else
                throw new ArgumentNullException();
        }

        #region Members
        ListViewItem _item;
        #endregion

        #region Properties
        
        public string Speed
        {
            get
            {
                return GetSubItem(1);
            }
        }

        public double SpeedNum
        {
            get
            {
                //hardcoded to MB
                double num = _item.SpeedByte;
                if (!AppSettings.SpeedUnits)
                    num *= 8;
                num /= Math.Pow(1024.0, 2.0);
                return (Math.Round(num, 2));
            }
        }

        public string IP
        {
            get
            {
                return GetSubItem(2);
            }
        }

        public string Message
        {
            get
            {
                return GetSubItem(3);
            }
        }

        public string Status
        {
            get
            {
                return GetSubItem(4);
            }
        }

        public ImageSource IconSource
        {
            get
            {
                switch (_item.ImageIndex)
                {
                    case 0:
                        return ImageSource.FromFile("ok.png");
                    case 1:
                        return ImageSource.FromFile("change.png");
                    case 2:
                        return ImageSource.FromFile("error.png");
                }
                return null;
            }
        }

        public DateTime Time
        { get { return _item.Time; } }

        string GetSubItem(int index)
        {
            if (_item.SubItems.Count > index)
                return _item.SubItems[index];
                return "";
        }

        public bool IsChange
        {
            get
            { return _item.IsStatusChanged; }
        }
        #endregion
    }
}
