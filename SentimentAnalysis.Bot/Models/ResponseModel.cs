using System.Collections.Generic;

namespace SentimentAnalysis.Bot.Models
{
	public class ResponseModel
	{
		/// <summary>
		/// 2 - Нейтрально
		/// 1 - Хорошо
		/// 0 - Плохо
		/// </summary>
		public int Prediction { get; set; }
		public Dictionary<LabelEnums, float> Scores { get; set; }
	}
}
