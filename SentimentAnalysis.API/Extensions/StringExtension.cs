using System;
using System.Linq;

namespace SentimentAnalysis.API.Extensions
{
	public static class StringExtension
	{
		public static string NormalizeString(this string str)
		{
			str = str.ToLower();
			str = str.Where(c => !char.IsPunctuation(c) && !char.IsNumber(c)).Aggregate("", (current, c) => current + c);
			str = string.Join(" ", str.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
			return str;
		}
	}
}
