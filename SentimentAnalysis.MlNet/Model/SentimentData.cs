using Microsoft.ML.Data;
using System;

namespace SentimentAnalysis.MlNet.Model
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string Message { get; set; }

        [LoadColumn(1), ColumnName(Constants.Label)]
        public int Result { get; set; }
    }
}
