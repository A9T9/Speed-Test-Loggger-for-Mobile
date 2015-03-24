using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger
{
    public class DownloadDataCompletedEventArgs : DownloadBaseEventArgs
    {
        public DownloadDataCompletedEventArgs(string state)
        {
            UserState = state;
        }

        public string UserState { get; private set; }
    }
}
