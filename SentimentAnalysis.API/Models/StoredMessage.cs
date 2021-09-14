using System.ComponentModel.DataAnnotations;

namespace SentimentAnalysis.API.Models
{
	public class StoredMessage
	{
		[Key]
		[Required]
		public int Id { get; set; }
		public string Message { get; set; }
		public int Result { get; set; }
		public int PredictResult { get; set; }
		public string Scores { get; set; }
	}
}
