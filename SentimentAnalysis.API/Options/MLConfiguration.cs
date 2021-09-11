using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentimentAnalysis.API.Options
{
    public class MLConfiguration
    {
        public string ModelName { get; set; }
        public string FilePath { get; set; }
        public bool WatchForChanges { get; set; }
    }
}
