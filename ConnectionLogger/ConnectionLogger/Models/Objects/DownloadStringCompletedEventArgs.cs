using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger
{
    public class DownloadStringCompletedEventArgs : DownloadBaseEventArgs
    {
        public DownloadStringCompletedEventArgs(string result)
        {
            Result = result;
        }
        public string Result { get; private set; }
    }
}
