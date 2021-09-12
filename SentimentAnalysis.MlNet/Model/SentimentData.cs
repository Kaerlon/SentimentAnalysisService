using Microsoft.ML.Data;

namespace SentimentAnalysis.MlNet.Model
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string Message { get; set; }

        [LoadColumn(1)]
        public int Label { get; set; }
    }
}
