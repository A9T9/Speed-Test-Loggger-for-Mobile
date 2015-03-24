using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger.ViewModels
{
    public class LogFileViewModel:BaseViewModel
    {
        public LogFileViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
