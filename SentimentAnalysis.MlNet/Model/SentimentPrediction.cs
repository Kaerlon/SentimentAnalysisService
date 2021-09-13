using Microsoft.ML.Data;

using System;

namespace SentimentAnalysis.MlNet.Model
{
	public class SentimentPrediction
	{
		public Int32 PredictLabel { get; set; }

		//public float Probability { get; set; }

		//public Vector<Single> Score { get; set; }

		[VectorType(3), ColumnName("Score")]
		public VBuffer<Single> Score { get; set; }
	}
}
