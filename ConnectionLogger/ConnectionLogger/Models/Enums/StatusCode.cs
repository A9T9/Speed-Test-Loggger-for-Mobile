﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger
{
    public enum StatusCode : int
    {
        None,
        Online,
        Offline,
        InvalidURL,
    }
}
