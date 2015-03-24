using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger
{
    public abstract class DownloadBaseEventArgs:EventArgs
    {
        public Exception Error { get; set; }
    }
}
