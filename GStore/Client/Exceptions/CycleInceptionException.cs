﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Exceptions
{
    class CycleInceptionException : System.Exception {
        public CycleInceptionException() : base() { }
    }
}
