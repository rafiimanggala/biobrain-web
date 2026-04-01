using System;

namespace BioBrain.AppResources
{
    public class UpdateData
    {
        public TimeSpan UpdateProcessMetric { get; set; }
        public TimeSpan DownloadProcessMetric { get; set; }
        public bool IsSuccess { get; set; }
        public int FilesFailed { get; set; }
    }
}