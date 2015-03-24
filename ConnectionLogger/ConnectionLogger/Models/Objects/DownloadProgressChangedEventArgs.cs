using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger
{
    
    public class DownloadProgressChangedEventArgs : EventArgs
    {
        public DownloadProgressChangedEventArgs(int bytesReceived)
        {
            BytesReceived = bytesReceived;
        }
        public int BytesReceived { get; private set; }
    }
}
