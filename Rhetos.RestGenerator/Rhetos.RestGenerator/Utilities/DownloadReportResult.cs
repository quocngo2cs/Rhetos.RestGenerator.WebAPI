
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhetos.WebApiRestGenerator.Utilities
{
    public class DownloadReportResult
    {
        public byte[] ReportFile { get; set; }
        public string SuggestedFileName { get; set; }
    }
}
