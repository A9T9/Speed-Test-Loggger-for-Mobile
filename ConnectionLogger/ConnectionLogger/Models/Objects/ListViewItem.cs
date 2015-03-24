using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger
{
    /// <summary>
    /// Simulating WinForms ListViewItem
    /// </summary>
    public class ListViewItem
    {
        #region Constructor
        public ListViewItem()
        {
            SubItems = new List<string>();
        }
        
        public ListViewItem(DateTime time, double speed):this()
        {
            var firstItem = Global.ConvertTime(time);
            SubItems.Add(firstItem);
            Time = time;
            SpeedByte = (long)speed;
        }
        #endregion

        #region Properties
        public List<string> SubItems { get; private set; }

        public int ImageIndex { get; set; }

        public long SpeedByte
        {
            get;
            private set;
        }

        public DateTime Time { get; private set; }

        public bool IsStatusOk
        {
            get
            { return ImageIndex == 0; }
        }
        public bool IsStatusChanged
        {
            get
            { return ImageIndex == 1; }
        }
        public bool IsStatusError
        {
            get
            { return ImageIndex == 2; }
        }
        #endregion

        #region Methods
        public void Refresh()
        {
            if (SubItems.Count > 5)
            {
                SubItems[0] = Global.ConvertTime(Time);
                SubItems[1] = Global.ConvertBytes(SpeedByte) + "/s";
            }
        }
        #endregion
    }

    
}
