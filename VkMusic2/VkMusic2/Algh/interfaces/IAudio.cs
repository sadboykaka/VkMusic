﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Algh.interfaces
{
    public interface IAudio
    {
        string Author { get; set; }
        string Name { get; set; }
        string Url { get; set; }

        string ID { get; set; }

        string Cover { get; set; }

        string From { get; set; }
    }
}
