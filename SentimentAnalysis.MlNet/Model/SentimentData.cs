using Microsoft.ML.Data;

namespace SentimentAnalysis.MlNet.Model
{
    public class SentimentData
    {
        [LoadColumn(0), ColumnName("Features")]
        public string Message { get; set; }
        [LoadColumn(1), ColumnName("Label")]
        public int Result { get; set; }
    }
}
