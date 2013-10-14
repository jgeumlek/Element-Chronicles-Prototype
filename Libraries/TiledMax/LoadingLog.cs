using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiledMax
{
    public class LoadingLog
    {
        public bool Successful { get; set; }
        public Exception Error { get; set; }

        public LoadingLog(bool successful, Exception ex)
        {
            Successful = successful;
            Error = ex;
        }
    }
}
