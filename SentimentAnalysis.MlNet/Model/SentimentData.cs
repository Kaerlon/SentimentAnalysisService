using Microsoft.ML.Data;

namespace SentimentAnalysis.MlNet.Model
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public int Id { get; set; }
        [LoadColumn(1), ColumnName("Features")]
        public string Message { get; set; }
        [LoadColumn(2), ColumnName("Label")]
        public int Result { get; set; }
    }
}
