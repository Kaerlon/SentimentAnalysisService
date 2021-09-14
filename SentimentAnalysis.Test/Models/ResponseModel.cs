using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalysis.Test.Models
{
	public class ResponseModel
	{
		public int Prediction { get; set; }
		public Dictionary<LabelEnums, float> Scores { get; set; }
	}
	public enum LabelEnums : byte
	{
		Negative = 0,
		Positive = 1,
		Neutral = 2,
	}
}
