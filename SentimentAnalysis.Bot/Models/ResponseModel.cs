using System.Collections.Generic;

namespace SentimentAnalysis.Bot.Models
{
	public class ResponseModel
	{
		public int Prediction { get; set; }
		public Dictionary<LabelEnums, float> Scores { get; set; }
	}
}
